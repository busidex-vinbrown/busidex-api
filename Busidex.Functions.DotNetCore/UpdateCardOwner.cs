using System;
using System.Threading.Tasks;
using Busidex.DataServices.DotNet;
using Busidex.DomainModels.DotNet.DTO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Busidex.Functions.DotNetCore
{
    public class UpdateCardOwner : FunctionBase
    {
        [FunctionName("UpdateCardOwner")]
        public async Task Run([QueueTrigger("update-owner", Connection = "AzureWebJobsStorage")]string data, ILogger log)
        {
            var connStr = Environment.GetEnvironmentVariable("busidexConnectionString");
            var model = JsonConvert.DeserializeObject<UpdateOwnerModel>(data);
            ICardRepository cardRepository = new CardRepository(connStr);
            await cardRepository.SaveCardOwner(model.CardId, model.OwnerId);
        }
    }
}
