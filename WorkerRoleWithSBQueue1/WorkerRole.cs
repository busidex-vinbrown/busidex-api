using System;
using System.Linq;
using System.Net;
using System.Threading;
using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace SharedCardQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QUEUE_NAME = "shared-card";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient _client;
        readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);


        public override void Run()
        {
            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _client.OnMessage(message =>
            {
                try
                {
                    using (var ctx = new BusidexDataContext())
                    {
                        ICardRepository cardRepository = new CardRepository(ctx);
                        IAccountRepository accountRepository = new AccountRepository(ctx);

                        var sharedCard = message.GetBody<SharedCard>();
                        if (sharedCard == null)
                        {
                            message.Complete();
                        }
                        else
                        {

                            var sendTo = accountRepository.GetUserAccountByEmail(sharedCard.Email);

                            // If we can't find the user account by email, see if they used the email on the user's card (if they have one)
                            if (sendTo == null)
                            {
                                var card = cardRepository.GetCardByEmail(sharedCard.Email);
                                if (card != null)
                                {
                                    sendTo = accountRepository.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
                                }
                            }

                            // try to find by phone number
                            //if (sendTo == null && !string.IsNullOrEmpty(sharedCard.PhoneNumber))
                            //{
                            //    sendTo = accountRepository.GetUserAccountByPhoneNumber(sharedCard.PhoneNumber);
                            //    if (sendTo != null && string.IsNullOrEmpty(sharedCard.Email))
                            //    {
                            //        sharedCard.Email = sendTo.BusidexUser.Email;
                            //        sharedCard.ShareWith = sendTo.UserId;
                            //    }
                            //}

                            if (sendTo == null || sharedCard.SendFrom == sharedCard.ShareWith)
                            {
                                // They're sharing with a person that doesn't have an account or sending themselves a test email. Send the invite email.
                                if (!string.IsNullOrEmpty(sharedCard.Email))
                                {
                                    SendSharedCardInvitation(sharedCard);
                                }
                                // DeQueue the message
                                message.Complete();
                            }
                            else
                            {
                                // This person has an account. Add the data to show the 'shared card' notification when the user logs in.

                                var theirBusidex = cardRepository.GetMyBusidex(sendTo.UserId, false);

                                // don't share the card if they already have it
                                
                                if (theirBusidex.All(b => b.CardId != sharedCard.CardId) || sharedCard.UseQuickShare)
                                {
                                    if (
                                        cardRepository.GetSharedCard(sharedCard.CardId, sharedCard.SendFrom,
                                            sendTo.UserId) == null)
                                    {
                                        cardRepository.SaveSharedCard(sharedCard);
                                    }
                                }

                                var eventSources = cardRepository.GetAllEventSources();
                                var eventSourceId =
                                    eventSources.Single(s => s.EventCode == EventSources.SHARE.ToString())
                                        .EventSourceId;

                                cardRepository.AddEventActivity(new EventActivity
                                {
                                    CardId = sharedCard.CardId,
                                    UserId = sendTo.UserId,
                                    ActivityDate = DateTime.UtcNow,
                                    EventSourceId = eventSourceId
                                });

                                SendSharedCard(sharedCard);

                                // DeQueue the message
                                message.Complete();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogError(ex, 0);
                }

            });

            _completedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 24;

            var runtimeUri = ServiceBusEnvironment.CreateServiceUri("sb",
                "busidex", string.Empty);

            var tokenProvider = TokenProvider
                .CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey",
                    "JwKsRwsFaQFTzUGWgCwSgoTkiT9vaHTgmR6MEvxy3Dk=");

            var mf = MessagingFactory.Create(runtimeUri,tokenProvider);

            _client = mf.CreateQueueClient(QUEUE_NAME);
            
            return base.OnStart();
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

        private void SendSharedCardInvitation(SharedCard model)
        {
            using (var ctx = new BusidexDataContext())
            {
                ICardRepository cardRepository = new CardRepository(ctx);
                cardRepository.SendSharedCardInvitation(model);
            }
        }

        private void SendSharedCard(SharedCard model)
        {
            using (var ctx = new BusidexDataContext())
            {
                ICardRepository cardRepository = new CardRepository(ctx);
                cardRepository.SendSharedCard(model);
            }
        }
    }
}
