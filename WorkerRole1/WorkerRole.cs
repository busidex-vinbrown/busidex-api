using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices;
using Busidex.Api.DataServices.Interfaces;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace WorkerRole1
{

    public class WorkerRole : RoleEntryPoint
    {

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient _client;
        readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);
        
        private const string QUEUE_NAME = "cards";

        public override bool OnStart()
        {
            bool result;
            try
            {
                // Set the maximum number of concurrent connections 
                ServicePointManager.DefaultConnectionLimit = 12;

                // Create the queue if it does not exist already
                //string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                //var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                //if (!namespaceManager.QueueExists(QUEUE_NAME))
                //{
                //    namespaceManager.CreateQueue(QUEUE_NAME);
                //}
                //_client = QueueClient.CreateFromConnectionString(connectionString, QUEUE_NAME);
                var runtimeUri = ServiceBusEnvironment.CreateServiceUri("sb",
                    "busidex", string.Empty);

                var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey",
                    "JwKsRwsFaQFTzUGWgCwSgoTkiT9vaHTgmR6MEvxy3Dk=");

                var mf = MessagingFactory.Create(runtimeUri,tokenProvider);

                _client = mf.CreateQueueClient(QUEUE_NAME);
            }
            catch (Exception ex)
            {
                LogError(ex, -1);
            }
            finally
            {
                result = base.OnStart();
            }
            return result;
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            _completedEvent.Set();
            base.OnStop();
        }

        private void LogError(Exception ex, long userId)
        {
            using (var ctx = new BusidexDataContext())
            {
                ICardRepository cardRepository = new CardRepository(ctx);
                cardRepository.SaveApplicationError(ex, userId);
            }
        }

        public override void Run()
        {
            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _client.OnMessage(message =>
            {
                long userId = 0;
                try
                {
                    using (var ctx = new BusidexDataContext())
                    {
                        ICardRepository cardRepository = new CardRepository(ctx);

                        var messageBody = message.GetBody<byte[]>();
                        if (messageBody == null)
                        {
                            message.Complete();
                        }
                        else
                        {
                            // deserialize the model
                            var jsModel = messageBody.DecompressToString();
                            var model = JsonConvert.DeserializeObject<AddOrEditCardModel>(jsModel);
							var card = Busidex.Api.DataAccess.DTO.Card.Clone(model);
                            var errors = new AddOrUpdateCardErrors();
                            userId = model.UserId;

                            // Add or update the card based on the card action
                            long cardId = 0;
                            if (model.Action == AddOrEditCardModel.CardAction.Add)
                            {
                                errors = cardRepository.AddCard(card, model.IsMyCard.GetValueOrDefault(),
                                    model.UserId, model.Notes, out cardId);
                            }
                            else if (model.Action == AddOrEditCardModel.CardAction.Edit)
                            {
                                cardId = card.CardId = model.CardId;

                                errors = cardRepository.EditCard(card, model.IsMyCard.GetValueOrDefault(),
                                    model.UserId, model.Notes);
                            }
                            else if (model.Action == AddOrEditCardModel.CardAction.ImageOnly)
                            {
                                cardId = card.CardId = model.CardId;
                            }

                            // mark the message as processed
                            if (cardId > 1 && errors.ErrorCollection.Count == 0)
                            {
                                if (model.Display == DisplayType.IMG)
                                {
                                    cardRepository.CardToFile(cardId, model.UpdateFrontImage, model.UpdateBackImage,
                                        model.FrontImage, model.FrontFileId.GetValueOrDefault(), model.FrontType,
                                        model.BackImage, model.BackFileId.GetValueOrDefault(), model.BackType, userId);
                                }

                                cardRepository.UpdateCardOrientation(cardId, model.FrontOrientation, model.BackOrientation);

                                // message is all set and safe to mark for removal
                                message.Complete();
                            }
                            else
                            {
                                var ex = new Exception("Card not updated",
                                    new Exception("Error count: " + errors.ErrorCollection.Count + ", CardId: " + cardId));

                                LogError(ex, userId);

                                // add it to the error queue and remove it from the new card queue
                                message.Complete();
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    LogError(sqlEx, userId);
                    // Leave the message queued so we can try again in a few seconds, we probably just lost the SQL connection
                }
                catch (Exception ex)
                {
                    LogError(ex, userId);
                }
            });
            _completedEvent.WaitOne();
        }


        public AddOrEditCardModel FromMessage(CloudQueueMessage m)
        {
            byte[] buffer = m.AsBytes;
            AddOrEditCardModel returnValue;
            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                var bf = new BinaryFormatter();
                returnValue = (AddOrEditCardModel) bf.Deserialize(ms);
            }
            return returnValue;
        }

        
    }
}
