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
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

//using Microsoft.ServiceBus.Messaging;

namespace EmailQueue
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        private const string QUEUE_NAME = "email";
        private const string EMAIL_USER = "azure_fb3e7fc30477e746ae332a32b77defde@azure.com";
        private const string EMAIL_PASSWORD = "g388ypfu";

        private NameValueCollection _section;

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient _client;
        readonly ManualResetEvent _completedEvent = new ManualResetEvent(false);

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QUEUE_NAME))
            {
                namespaceManager.CreateQueue(QUEUE_NAME);
            }

            _section = (NameValueCollection)ConfigurationManager.GetSection("emailInfo");

            // Initialize the connection to Service Bus Queue
            _client = QueueClient.CreateFromConnectionString(connectionString, QUEUE_NAME);
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

                        var email = SendGrid.Mail.GetInstance();
                        email.From = new MailAddress(_section["Support"], "Busidex");
                        email.AddTo(model.Email);
                        email.Subject = model.EmailTemplate != null ? model.EmailTemplate.Subject : "(no subject)";
                        email.Html = model.EmailTemplate != null ? model.EmailTemplate.Body : "";

                        var transportInstance =
                            SendGrid.Transport.SMTP.GetInstance(
                                new NetworkCredential(EMAIL_USER, EMAIL_PASSWORD));
                        transportInstance.DeliverAsync(email);

                        message.Complete();
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
