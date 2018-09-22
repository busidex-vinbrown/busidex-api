using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Json;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex4.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace Busidex4.Controllers
{
    [ErrorLogger(View = "Error")]
    public class BaseController : Controller
    {

        protected readonly IAccountRepository _accountRepository;
        protected readonly ICardRepository _cardRepository;

        public BaseController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            GetBaseViewData();
        }

        protected const string BING_URL = "http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}";

        protected string GetCurrentView(string v)
        {
            const Busidex.DAL.ViewType DEFAULT_VIEW = Busidex.DAL.ViewType.Details;
            string currentView = Enum.GetName(typeof(Busidex.DAL.ViewType), DEFAULT_VIEW);
            if (!string.IsNullOrEmpty(v))
            {
                if (Enum.GetNames(typeof(Busidex.DAL.ViewType)).Any(view => v.ToLower() == view.ToLower()))
                {
                    currentView = v.ToLower();
                }
            }
            if (currentView != null)
            {
                ViewData["ViewType"] = Enum.Parse(typeof(Busidex.DAL.ViewType), currentView, true);
                return currentView;
            }
            return null;
        }

        protected void GetBaseViewData()
        {
            ViewData["UserID"] = -1;

            var userId = GetUserId();
            ViewData["UserID"] = userId;

            var sharedCards = _cardRepository.GetSharedCards(userId);
            if (sharedCards.Count > 0)
            {
                var sharedCard = sharedCards.First();
                ViewBag.SharedFrom = sharedCard.SendFromEmail;
            }
            ViewBag.SharedCards = sharedCards;

            var user = Membership.GetUser();
            ViewBag.IsAdmin = (user != null) && Roles.IsUserInRole(user.UserName, "Administrator");
            ViewBag.CurrentUser = user;
        }

        protected Size NewImageSize(int OriginalHeight, int OriginalWidth, double FormatSize)
        {
            Size NewSize;
            double tempval;

            if (OriginalHeight > FormatSize && OriginalWidth > FormatSize)
            {
                if (OriginalHeight > OriginalWidth)
                    tempval = FormatSize / Convert.ToDouble(OriginalHeight);
                else
                    tempval = FormatSize / Convert.ToDouble(OriginalWidth);

                NewSize = new Size(Convert.ToInt32(tempval * OriginalWidth), Convert.ToInt32(tempval * OriginalHeight));
            }
            else
                NewSize = new Size(OriginalWidth, OriginalHeight); return NewSize;
        }

        protected Tuple<double, double> ProcessResponse(Response locationsResponse)
        {
            int locNum = locationsResponse.ResourceSets[0].Resources.Length;

            //Get all locations that have a MatchCode=Good and Confidence=High
            double lat = 0, lon = 0;

            //for (int i = 0; i < locNum; i++)
            //{
            //    var location = locationsResponse.ResourceSets[0].Resources[i];
            //    //if (location.Confidence == "High")
            //    //{
            //    lat = location.Point.Coordinates[0];
            //    lon = location.Point.Coordinates[1];
            //    break;
            //    //}
            //}
            if (locNum > 0)
            {
                var location = locationsResponse.ResourceSets[0].Resources[0];
                lat = location.Point.Coordinates[0];
                lon = location.Point.Coordinates[1];
            }
            return new Tuple<double, double>(lat, lon);
        }

        protected Response MakeRequest(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    var jsonResponse = objResponse as Response;
                    return jsonResponse;

                }
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                return null;
            }
        }

        protected void SendEmail(Communication communication)
        {
            var con = ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString;

            var storageAccount = CloudStorageAccount.Parse(con);

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            var queue = queueClient.GetQueueReference("email");

            // Create the queue if it doesn't already exist.
            queue.CreateIfNotExist();

            // Create a message and add it to the queue.
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                Formatting = Formatting.Indented
            };

            var jsMessage = JsonConvert.SerializeObject(communication, settings);
            var message = new CloudQueueMessage(jsMessage);

            queue.AddMessage(message);
        }

        protected long GetUserId()
        {
            long userId = 0;
            MembershipUser user = Membership.GetUser();

            if (user != null && user.ProviderUserKey != null)
            {
                userId = (long)user.ProviderUserKey;
            }
            return userId;
        }
    }
}
