using System;
using System.Linq;
using System.Threading.Tasks;
using Busidex.DataServices.DotNet;
using Busidex.DomainModels.DotNet.DTO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Busidex.Functions.DotNetCore
{
    public class SendSharedCardFunction : FunctionBase
    {
        [FunctionName("SendSharedCard")]
        public async Task Run([QueueTrigger("shared-card", Connection = "AzureWebJobsStorage")]string sharedCardJson, ILogger log)
        {
            try
            {
                var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
                
                ICardRepository cardRepository = new CardRepository(connStr);
                IAccountRepository accountRepository = new AccountRepository(connStr);

                var sharedCard = JsonConvert.DeserializeObject<SharedCard>(sharedCardJson);
                var sendTo = accountRepository.GetUserAccountByEmail(sharedCard.Email);

                // If we can't find the user account by email, see if they used the email on the user's card (if they have one)
                if (sendTo == null)
                {
                    var card = cardRepository.GetCardByEmail(sharedCard.Email);
                    if (card != null)
                    {
                        sendTo = await accountRepository.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
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

                    var eventSources = await cardRepository.GetAllEventSources();
                    var eventSourceId =
                        eventSources.Single(s => s.EventCode == EventSources.SHARE.ToString())
                            .EventSourceId;

                    await cardRepository.AddEventActivity(new EventActivity
                    {
                        CardId = sharedCard.CardId,
                        UserId = sendTo.UserId,
                        ActivityDate = DateTime.UtcNow,
                        EventSourceId = eventSourceId
                    });

                    SendSharedCard(sharedCard);
                }
                
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                await LogError(ex, 0);
            }
        }

        private static void SendSharedCardInvitation(SharedCard model)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
          
            ICardRepository cardRepository = new CardRepository(connStr);
            cardRepository.SendSharedCardInvitation(model);
        }

        private static void SendSharedCard(SharedCard model)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
            
            ICardRepository cardRepository = new CardRepository(connStr);
            cardRepository.SendSharedCard(model);
        } 
    }
}
