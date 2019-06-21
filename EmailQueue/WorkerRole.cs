using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices;
using Busidex.Api.DataServices.Interfaces;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;
//using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SendGrid;
using SendGrid.Helpers.Mail;

//using Microsoft.ServiceBus.Messaging;

namespace EmailQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        private const string QUEUE_NAME = "email";
        private const string EMAIL_USER = "azure_fb3e7fc30477e746ae332a32b77defde@azure.com";
        private const string EMAIL_PASSWORD = "Guvw7_WuvuMomu";
        private const string API_KEY = "<hidden>";

        private NameValueCollection _section;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient _client;
        readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            
            _section = (NameValueCollection)ConfigurationManager.GetSection("emailInfo");

            // Initialize the connection to Service Bus Queue
            var runtimeUri = ServiceBusEnvironment.CreateServiceUri("sb",
                             "busidex", string.Empty);

            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey",
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

        public override void Run()
        {
            _client.OnMessage(message => 
            {
                try
                {
                    var model = message.GetBody<Communication>();

                    if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@") || !model.Email.Contains("."))
                    {
                        model = null;
                    }

                    if (model == null)
                    {
                        message.Complete();
                    }
                    else
                    {
                        var from = new SendGrid.Helpers.Mail.EmailAddress(_section["Support"], "Busidex");
                        var to = new SendGrid.Helpers.Mail.EmailAddress(model.Email, model.Email);
                        var subject = model.EmailTemplate != null ? model.EmailTemplate.Subject : "(no subject)";
                        var htmlContent = model.EmailTemplate != null ? model.EmailTemplate.Body : "";
                        var plainTextContent = htmlContent;
                        var client = new SendGridClient(API_KEY);
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                        client.SendEmailAsync(msg).ContinueWith(response => {
                            if (response.Result.StatusCode != HttpStatusCode.Accepted)
                            {
                                LogError(new Exception("Response was: " + response.Result.StatusCode + msg.ToString()), 0);
                            }
                            message.Complete();
                        });        
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, 0);
                }
            });

            _completedEvent.WaitOne();

        }
    }
}
