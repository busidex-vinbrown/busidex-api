using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;


namespace Busidex.Api.Controllers
{   

    public class BaseApiController : ApiController
    {        
        protected readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>();
        protected const long DEMO_CARD_ID = 1;
        protected ICardRepository _cardRepository;
        internal static readonly string REQ_PROP_CLIENT_APP_NAME = "BaseApiController.AppName";
        internal static readonly string REQ_PROP_MEMBER_ID = "BaseApiController.MemberID";
        internal static string _connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private static readonly Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden

        protected static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        protected long ValidateUser()
        {
            long userId = 0;
            try
            {
                var header = Request.Headers.FirstOrDefault(h => h.Key.ToLowerInvariant().Equals("x-authorization-token"));
                var val = header.Value != null ? header.Value.FirstOrDefault() : string.Empty;

                if (!string.IsNullOrEmpty(val))
                {
                    var userIdStr = Encoding.ASCII.GetString(Convert.FromBase64String(val));
                    long.TryParse(userIdStr, out userId);
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
            }
            return userId;
        }

        protected string EncodeString(string newString)
        {
            Configuration cfg =
                WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            var machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
            var hash = new HMACSHA1 { Key = HexToByte(machineKey.ValidationKey) };
            var encodedString = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(newString)));
            return encodedString;
        }

        protected byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        protected string RandomString(int size)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        protected static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        protected static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        protected void SendEmail(Communication communication)
        {

            // Create the queue client.
            QueueClient queueClient = QueueClient.CreateFromConnectionString(_connectionString, "email");

            var message = new BrokeredMessage(communication)
            {
                Label = communication.Email,
                TimeToLive = new TimeSpan(7, 0, 0, 0)
            };

            queueClient.Send(message);

        }
        
    }
}
