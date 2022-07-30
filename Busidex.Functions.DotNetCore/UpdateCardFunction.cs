using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Busidex.DataServices.DotNet;
using Busidex.DomainModels.DotNet.DTO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Busidex.Functions.DotNetCore
{
    public class UpdateCardFunction : FunctionBase
    {
        [FunctionName("UpdateCard")]
        public async Task Run([QueueTrigger("card-update", Connection = "AzureWebJobsStorage")]string cardRef, ILogger log)
        {
            long userId = 0;
            try
            {
                log.LogInformation("Starting card update...");

                var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
               // log.LogInformation("BusidexConnectionString: " + connStr);
                
                ICardRepository cardRepository = new CardRepository(connStr);
                var model = await GetModelFromStorage(cardRef);

                var card = Card.Clone(model);
                userId = model.UserId;

                // Add or update the card based on the card action
                long cardId = 0;
                if (model.Action == AddOrEditCardModel.CardAction.Add)
                {
                    cardId = await cardRepository.AddCard(card, model.IsMyCard.GetValueOrDefault(),
                        model.UserId, model.Notes);
                }
                else if (model.Action == AddOrEditCardModel.CardAction.Edit)
                {
                    cardId = card.CardId = model.CardId;

                    await cardRepository.EditCard(card, model.IsMyCard.GetValueOrDefault(),
                        model.UserId, model.Notes);

                    await cardRepository.CardToFile(card.CardId, true, true,
                            card.FrontImage, card.FrontFileId.GetValueOrDefault(), card.FrontType,
                            card.BackImage, card.BackFileId.GetValueOrDefault(), card.BackType, userId);

                    await cardRepository.UpdateCardOrientation(cardId, model.FrontOrientation, model.BackOrientation);

                }
                else if (model.Action == AddOrEditCardModel.CardAction.ImageOnly)
                {
                    cardId = card.CardId = model.CardId;
                }

                // mark the message as processed
                //if (cardId > 1)
                //{
                //    if (model.Display == DisplayType.IMG)
                //    {
                //        await cardRepository.CardToFile(cardId, model.UpdateFrontImage, model.UpdateBackImage,
                //            model.FrontImage, model.FrontFileId.GetValueOrDefault(), model.FrontType,
                //            model.BackImage, model.BackFileId.GetValueOrDefault(), model.BackType, userId);
                //    }

                //    await cardRepository.UpdateCardOrientation(cardId, model.FrontOrientation, model.BackOrientation);
                //}
                //else
                //{
                //    var ex = new Exception($"Card not updated. CardId {cardId}");

                //    await LogError(ex, userId);
                //}
                log.LogInformation("Card update complete!");
                
            }
            catch (Exception ex)
            {
                log.LogError(0, ex, ex.Message);
                await LogError(ex, userId);
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
    }
}
