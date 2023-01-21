using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Busidex.Functions.DotNetCore
{
    public class KeepAlive : FunctionBase
    {
        [FunctionName("KeepAlive")]
        public static async Task KeepAliveFunction([TimerTrigger("0 */2 * * * *")] TimerInfo timer, ILogger log)
        {
            const string QUEUE_NAME = "ping";
            var connectionString = Environment.GetEnvironmentVariable("BusidexQueuesConnectionString");
            var queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
            queueClient.CreateIfNotExists();
            var now = DateTime.UtcNow;
            var message = $"Received ping request at {now}";
            var msg = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(message));
            await queueClient.SendMessageAsync(msg);
        }

        [FunctionName("Ping")]
        public async Task Run([QueueTrigger("ping", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {            
            Console.WriteLine(message);
            log.LogInformation(message);
        }
    }
}
