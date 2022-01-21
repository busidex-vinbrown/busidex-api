using System;
using System.Threading.Tasks;
using Busidex.DomainModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace Busidex.Functions.DotNetCore
{
    public class SendEmailFunction
    {
        [FunctionName("SendEmail")]
        public async Task Run(
            [QueueTrigger("email", Connection = "AzureWebJobsStorage")] string communication, ILogger log,
            [SendGrid(ApiKey = "SENDGRID_API_KEY")] IAsyncCollector<SendGridMessage> messageCollector)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<Communication>(communication);

                if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@") || !model.Email.Contains("."))
                {
                    model = null;
                }

                var from = new SendGrid.Helpers.Mail.EmailAddress(Environment.GetEnvironmentVariable("EmailSupport"), "Busidex");
                var to = new SendGrid.Helpers.Mail.EmailAddress(model.Email, model.Email);
                var subject = model.EmailTemplate != null ? model.EmailTemplate.Subject : "(no subject)";
                var htmlContent = model.EmailTemplate != null ? model.EmailTemplate.Body : "";
                var plainTextContent = htmlContent;

                var message = new SendGridMessage();
                message.AddTo(to);
                message.AddContent("text/html", model.EmailTemplate != null ? model.EmailTemplate.Body : "");
                message.PlainTextContent = htmlContent;
                message.SetFrom(from);
                message.SetSubject(subject);

                await messageCollector.AddAsync(message);

            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
