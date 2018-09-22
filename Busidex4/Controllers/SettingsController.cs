using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex4.Models;
//using DotNetOpenAuth.Messaging;
//using DotNetOpenAuth.OAuth2;
//using Google.GData.Client;
//using Google.Contacts;

namespace Busidex4.Controllers
{
    [ErrorLogger(View = "Error")]
    public class SettingsController : BaseController
    {

        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository, ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            if (settingsRepository == null) throw new ArgumentNullException("settingsRepository");
            _settingsRepository = settingsRepository;
        }

        //public ActionResult SyncGoogleContacts()
        //{
        //    var googleInfo = (GoogleInfo)System.Configuration.ConfigurationManager.GetSection("GoogleInfo");

        //    string clientId = googleInfo.ClientId;
        //    string clientSecret = googleInfo.ClientSecret;
        //    string redirectUri = googleInfo.RedirectURIs;
        //    var server = new AuthorizationServerDescription
        //    {
        //        AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth"),
        //        TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token"),
        //        ProtocolVersion = ProtocolVersion.V20,
        //    };
        //    var scope = new string[] { googleInfo.Scope };
        //    var consumer = new WebServerClient(server, clientId, clientSecret);
        //    consumer.RequestUserAuthorization(scope, new Uri(redirectUri));

        //    OutgoingWebResponse response = consumer.PrepareRequestUserAuthorization(scope, new Uri(redirectUri));
        //    return response.AsActionResult();
        //}

        [AllowAnonymous]
        public ActionResult oAuth2Callback(string state, string code)
        {

            return RedirectToAction("GoogleContacts", "Busidex", new { state, code });
        }

        [Authorize]
        public ActionResult Index()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    BusidexUser user = _settingsRepository.GetBusidexUserById((long)providerUserKey);
                    Setting s = user.Settings ?? _settingsRepository.AddDefaultUserSetting(user);
                    IEnumerable<Page> pages = _settingsRepository.GetAllSitePages();
                    ViewBag.SystemPages = pages;
                    ViewBag.UserStartUpPage = pages.SingleOrDefault(p => p.PageId == user.Settings.StartPage);


                    //var scope = new List<string>
                    //                {
                    //                   G GoogleScope.ImapAndSmtp.Name,
                    //                    GoogleScope.EmailAddressScope.Name
                    //                };

                    return View(s);
                }
            }
            return null;
        }

        [Authorize]
        public ActionResult Save()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    BusidexUser user = _settingsRepository.GetBusidexUserById((long)providerUserKey);
                    Setting s = _settingsRepository.GetUserSetting(user) ?? new Setting();
                    TryUpdateModel(s);

                    if (s.SettingsId == 0)
                    {
                        _settingsRepository.SaveSetting(s);
                    }
                    else
                    {
                        _settingsRepository.UpdateSetting(s);
                    }

                }
            }

            return RedirectToAction("Index");
        }
    }
}
