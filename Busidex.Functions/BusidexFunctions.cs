using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Busidex.Functions
{
    public class BusidexFunctions
    {
        #region Card Update
        [FunctionName("UpdateCard")]
        public static async Task UpdateCard(
            [QueueTrigger("card-update", Connection = "AzureWebJobsStorage")] 
            string cardRef, ILogger log)
        {
            long userId = 0;
            try
            {
                var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
                log.LogInformation("BusidexConnectionString: " + connStr);

                using (var ctx = new BusidexDataContext(connStr))
                {
                    ICardRepository cardRepository = new CardRepository(ctx, connStr);
                    var model = await GetModelFromStorage(cardRef);

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
                    }
                    else
                    {
                        var ex = new Exception("Card not updated",
                            new Exception("Error count: " + errors.ErrorCollection.Count + ", CardId: " + cardId));

                        LogError(ex, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(0, ex, ex.Message);
                LogError(ex, userId);
            }
        }

        private static void LogError(Exception ex, long userId)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");

            using (var ctx = new BusidexDataContext(connStr))
            {
                ICardRepository cardRepository = new CardRepository(ctx, connStr);
                cardRepository.SaveApplicationError(ex, userId);
            }
        }

        private static async Task<AddOrEditCardModel> GetModelFromStorage(string cardRef)
        {
            var connStr = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var blobServiceClient = new BlobServiceClient(connStr);
            var containerClient = blobServiceClient.GetBlobContainerClient("card-update");
            var blobClient = containerClient.GetBlobClient(cardRef);

            var response = await blobClient.DownloadAsync();
            var streamReader = new StreamReader(response.Value.Content);
            var json = streamReader.ReadToEnd();

            var returnValue = JsonConvert.DeserializeObject<AddOrEditCardModel>(json);

            return returnValue;
        }

        #endregion

        #region Shared Cards
        [FunctionName("SendSharedCard")]
        public static void SendSharedCard(
            [QueueTrigger("shared-card", Connection = "AzureWebJobsStorage")]
            string sharedCardJson, ILogger log)
        {
            try
            {
                var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");

                using (var ctx = new BusidexDataContext(connStr))
                {
                    ICardRepository cardRepository = new CardRepository(ctx);
                    IAccountRepository accountRepository = new AccountRepository(ctx);

                    var sharedCard = JsonConvert.DeserializeObject<SharedCard>(sharedCardJson);
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

                    if (sendTo == null || sharedCard.SendFrom == sharedCard.ShareWith)
                    {
                        // They're sharing with a person that doesn't have an account or sending themselves a test email. Send the invite email.
                        if (!string.IsNullOrEmpty(sharedCard.Email))
                        {
                            SendSharedCardInvitation(sharedCard);
                        }
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
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, 0);
            }
        }

        private static void SendSharedCardInvitation(SharedCard model)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");

            using (var ctx = new BusidexDataContext(connStr))
            {
                ICardRepository cardRepository = new CardRepository(ctx);
                cardRepository.SendSharedCardInvitation(model);
            }
        }

        private static void SendSharedCard(SharedCard model)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");

            using (var ctx = new BusidexDataContext(connStr))
            {
                ICardRepository cardRepository = new CardRepository(ctx);
                cardRepository.SendSharedCard(model);
            }
        }
        #endregion
    }
}
