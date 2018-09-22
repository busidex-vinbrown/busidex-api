using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex.Providers;

namespace Busidex4.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            
        }        

        public enum HomeContentKeys
        {
            HomeMissionStatement,
            HomeFindACard,
            HomeAddACard,
            HomeMyBusidex,
            HomeElevatorPitch,
            HomeMyBusiGroups
        }

        public ActionResult Index()
        {

            var user = Membership.GetUser();
            if (user != null && user.ProviderUserKey != null)
            {
            }
            ViewBag.UserName = user != null ? user.UserName : "";

            var content =
                Enum.GetNames(typeof(HomeContentKeys))
                    .ToDictionary(key => (HomeContentKeys)Enum.Parse(typeof(HomeContentKeys), key),
                                  key => ContentProvider.GetContent(key));

            ViewBag.Content = content;
            ViewBag.HomePage = true;

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public string GetCardCount(int rand)
        {
            var bdc = new BusidexDataContext();
            return bdc.GetCardCount().ToString(CultureInfo.InvariantCulture);
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsOfService()
        {
            return View();
        }

        public PartialViewResult ShowMeHow(string helpFile)
        {
            var path = ConfigurationManager.AppSettings["videoCDNPath"];
            ViewBag.HelpFile = path + helpFile;
            return PartialView("_ShowMeHow");
        }
    }
}
