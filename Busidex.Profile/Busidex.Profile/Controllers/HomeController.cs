using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Busidex.Profile.DataAccess;
using Busidex.Profile.Services;

namespace Busidex.Profile.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string name)
        {
            //var base64EncodedBytes = Convert.FromBase64String(encoding);
            //var searchText = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            var card = CardService.GetCardBySEOName(name) ?? new CardDetailModel();

            if (card.FrontFileId == null)
            {
                return new RedirectResult("https://www.busidex.com");
            }
            card.PhoneNumbers = card.PhoneNumbers ?? new List<PhoneNumber>();
            card.Url = FixUrl(card.Url);

            
            return View(card);
        }

        string FixUrl(string url)
        {
            const string PREFIX = "http://";
            const string SECURE_PREFIX = "https://";

            if(!string.IsNullOrEmpty(url))
            {
                //url = url.Replace(".", string.Empty);

                if (!url.StartsWith(PREFIX) && !url.StartsWith(SECURE_PREFIX))
                {
                    url = PREFIX + url;
                }
            }
            return url;
        }

        public ActionResult SiteMap()
        {
            var referrer = Request.UrlReferrer;
            if (referrer != null)
            {
                return new RedirectResult("https://www.busidex.com");
            }

            var model = CardService.GetAllCards().Where(c=>c.CardType == CardType.Professional).OrderBy(c=>c.Name).ToList();
            model.ForEach(card => { card.Url = FixUrl(card.Url); });
            return View(model);
        }
    }
}