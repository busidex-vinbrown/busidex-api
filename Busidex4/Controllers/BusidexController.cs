using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using System.Web.Security;
//using Busidex4.App_Start;
using Busidex4.Models;
//using DotNetOpenAuth.ApplicationBlock;
//using DotNetOpenAuth.ApplicationBlock;
//using DotNetOpenAuth.OAuth;
//using DotNetOpenAuth.ApplicationBlock;
using Newtonsoft.Json;
//using Google.GData.Client;
//using Google.Contacts;

//using DotNetOpenAuth.ApplicationBlock;

namespace Busidex4.Controllers
{

    [ErrorLogger]
    public class BusidexController : BaseController
    {

        public BusidexController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            
        }

        [Authorize]
        public void PrimeMyBusidex()
        {
            var userId = GetUserId();
            _cardRepository.GetMyBusidex(userId, false);
        }

        [Authorize]
        public ActionResult Mine()
        {
            var userId = GetUserId();
            List<UserCard> list = _cardRepository.GetMyBusidex(userId, false).ToList();
            var sorted = new List<UserCard>();
            sorted.AddRange(list.Where(c => c.Card.OwnerId == userId));
            sorted.AddRange(
                list
                    .Where(c => c.Card.OwnerId != userId)
                    .OrderByDescending(c => c.Card.OwnerId.GetValueOrDefault())
                    .ThenBy(c => c.Card.CompanyName)
                    .ThenBy(c => c.Card.Name)
                    .ToList());


            var allCards = sorted.Select(userCard => userCard.Card).ToList();
            List<string> allTags = (from cards in allCards
                from tag in cards.Tags
                select tag.Text).ToList();

            var tags = (from tag in allTags
                group tag by tag
                into t
                select new {key = t.First(), Value = t.Count()})
                .ToDictionary(t => t.key, t => t.Value);

            string currentView = Busidex.DAL.ViewType.Details.ToString();//  GetCurrentView(v);

            var model = new MyBusidex
                        {
                            Busidex = sorted,
                            IsLoggedIn = User.Identity.IsAuthenticated,
                            TagCloud = tags,
                            CurrentView = currentView,
                            CardCount = list.Count
                        };
            return View(model);

        }

        public JsonResult SaveCardNotes(long id, string notes)
        {
            _cardRepository.SaveCardNotes(id, notes);

            return Json(new { Success = true });
        }

        public void SendSharedCards(string email, string cards)
        {
            try
            {
              

                string[] sCardIds = cards.Split(',');
                var sendTo = _accountRepository.GetUserAccountByEmail(email);
                if (sendTo != null)
                {
                    var cardIds = sCardIds.Select(long.Parse).ToList();
                    var userId = GetUserId();
                    var sharedCards = (from cardId in cardIds
                                       where userId > 0
                                       select new SharedCard
                                                  {
                                                      CardId = cardId,
                                                      Email = email,
                                                      Accepted = false,
                                                      SendFrom = userId,
                                                      ShareWith = sendTo.UserId,
                                                      SharedDate = DateTime.Now
                                                  }).ToList();

                    _cardRepository.SaveSharedCards(sharedCards);

                }
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
            }
        }

        public void RelateCards(long ownerId, long relatedCardId)
        {
            _cardRepository.RelateCards(ownerId, relatedCardId);
        }

        public void UnRelateCards(long ownerId, long relatedCardId)
        {
            _cardRepository.UnRelateCards(ownerId, relatedCardId);
        }

        public string MyBusidexJSON(long id)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var results = _cardRepository.GetMyBusidex(id, false).ToList();
            var sorted = new List<UserCard>();
            sorted.AddRange(results.Where(c => c.Card.OwnerId == id));
            sorted.AddRange(
                results
                    .Where(c => c.Card.OwnerId != id)
                    .OrderByDescending(c => c.Card.OwnerId.GetValueOrDefault())
                    .ThenBy(c => c.Card.CompanyName)
                    .ThenBy(c => c.Card.Name)
                    .ToList());

            var cardList = sorted.Select(cards => new MobileCardSmall
                            {
                                FrontFileId = cards.Card.FrontFileId != null ? cards.Card.FrontFileId.ToString() + "." + cards.Card.FrontType : string.Empty,
                                Name = cards.Card.Name ?? string.Empty,
                                Email = cards.Card.Email ?? string.Empty,
                                TagList = string.IsNullOrEmpty(cards.Card.TagList) ? "." : cards.Card.TagList.ToLower(),
                                Company =  cards.Card.CompanyName ?? string.Empty,
                                PhoneNumbers = new List<string>(from p in cards.Card.PhoneNumbers select !string.IsNullOrEmpty(p.Number) ? p.Number : string.Empty)
                            }).ToList();

            return JsonConvert.SerializeObject(cardList);
        }

        public string SyncData(long id)
        {
            return MyBusidexJSON(id);
        }

        public ActionResult Suggestions()
        {
            List<Suggestion> model = _accountRepository.GetAllSuggestions();
            return View(model);
        }

        public ActionResult AddSuggestion(string summary, string details)
        {
            var suggestion = new Suggestion();
            if (TryUpdateModel(suggestion))
            {
                suggestion.CreatedBy = GetUserId();
                suggestion.Votes = 1;
                if (!string.IsNullOrEmpty(suggestion.Summary))
                {
                    _accountRepository.AddNewSuggestion(suggestion);
                }
            }

            return RedirectToAction("Suggestions");
        }

        public ActionResult AddVote(int? suggestionId = 0)
        {
            if (suggestionId.GetValueOrDefault() > 0)
            {
                _accountRepository.UpdateSuggestionVoteCount(suggestionId.GetValueOrDefault());
            }
            return RedirectToAction("Suggestions");
        }

        public PartialViewResult GoogleContacts(string state, string code)
        {

            //AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(
            //    Url.Action("GoogleContacts", new { ReturnUrl = "Mine" }));

            //if (result.ExtraData.Keys.Contains("accesstoken"))
            //{
            //    Session["googleAccessToken"] = result.ExtraData["accesstoken"];
            //}

            //if (OAuthWebSecurity.Login(
            //    result.Provider,
            //    result.ProviderUserId,
            //    createPersistentCookie: false))
            //{
            //    Session["googleAccessToken"] = result.ExtraData["accesstoken"];
            //}

            //var rs = new RequestSettings("busidex", "vinbrown2@gmail.com",
            //                             "ride9737")
            //             {

            //                 AutoPaging = true
            //             };




            var googleInfo = (GoogleInfo)System.Configuration.ConfigurationManager.GetSection("GoogleInfo");


            string consumerKey = googleInfo.ClientId;
            string consumerSecret = googleInfo.ClientSecret;



            //XDocument contactsDocument = GoogleConsumer.GetContacts(consumerKey, consumerSecret);

            //var contacts = from entry in contactsDocument.Root.Elements(XName.Get("entry", "http://www.w3.org/2005/Atom"))
            //               select new
            //               {
            //                   Name = entry.Element(XName.Get("title", "http://www.w3.org/2005/Atom")).Value,
            //                   Email = entry.Element(XName.Get("email", "http://schemas.google.com/g/2005")).Attribute("address").Value
            //               };
            //var parameters = new OAuth2Parameters
            //                     {
            //                         ClientId = googleInfo.ClientId,
            //                         ClientSecret = googleInfo.ClientSecret,
            //                         Scope = googleInfo.Scope,
            //                         AccessCode = Server.HtmlDecode(Server.UrlDecode(code)),
            //                         AccessToken = googleInfo.AccessToken,
            //                         //RefreshToken = Server.HtmlDecode(Server.UrlDecode(code)),
            //                         RedirectUri = googleInfo.RedirectURIs
            //                     };
            ////string url = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
            //string queryStringFormat = @"code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";
            //string postcontents = string.Format(queryStringFormat
            //                                   , HttpUtility.UrlEncode(parameters.AccessCode)
            //                                   , HttpUtility.UrlEncode(googleInfo.ClientId)
            //                                   , HttpUtility.UrlEncode(googleInfo.ClientSecret)
            //                                   , HttpUtility.UrlEncode(googleInfo.RedirectURIs));
            //var request = (HttpWebRequest)HttpWebRequest.Create("https://accounts.google.com/o/oauth2/token");
            //request.Method = "POST";
            //byte[] postcontentsArray = Encoding.UTF8.GetBytes(postcontents);
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = postcontentsArray.Length;
            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(postcontentsArray, 0, postcontentsArray.Length);
            //    requestStream.Close();
            //    WebResponse response = request.GetResponse();
            //    using (Stream responseStream = response.GetResponseStream())
            //    using (var reader = new StreamReader(responseStream))
            //    {
            //        string responseFromServer = reader.ReadToEnd();
            //        reader.Close();
            //        responseStream.Close();
            //        response.Close();
            //        var auth = JsonConvert.DeserializeObject<AuthResponse>(responseFromServer);
            //        parameters.AccessToken = auth.access_token;
            //        parameters.RefreshToken = auth.access_token;
            //    }
            //}
            ////var rs = new RequestSettings("busidex", "lizzabethbrown@gmail.com", "bvl252706");
            //var rs = new RequestSettings("busidex", parameters);
            //// AutoPaging results in automatic paging in order to retrieve all contacts

            //var cr = new ContactsRequest(rs);
            var cards = new List<Card>();

            //Feed<Contact> f = cr.GetContacts();
            //foreach (Contact entry in f.Entries)
            //{
            //    if (entry.Name != null && !string.IsNullOrEmpty(entry.Name.FullName))
            //    {
            //        var card = new Card
            //                       {
            //                           Name = entry.Name.FullName,
            //                           Email = (from email in entry.Emails
            //                                    select email.Address).FirstOrDefault()
            //                       };

            //        const int WORK = 1;
            //        const int HOME = 2;
            //        const int OTHER = 7;
            //        card.PhoneNumbers = new List<PhoneNumber>();
            //        foreach (var phonenumber in entry.Phonenumbers)
            //        {
            //            card.PhoneNumbers.Add(new PhoneNumber
            //                                      {
            //                                          Number = phonenumber.Value,
            //                                          PhoneNumberType = new PhoneNumberType
            //                                                                {
            //                                                                    PhoneNumberTypeId = phonenumber.Work ? WORK
            //                                                                    : phonenumber.Home ? HOME
            //                                                                    : OTHER,
            //                                                                    Name = phonenumber.Work ? "Work"
            //                                                                    : phonenumber.Home ? "Home"
            //                                                                    : "Other"
            //                                                                }
            //                                      });
            //        }
            //        cards.Add(card);

            //    }

            //}

            return PartialView("_GoogleContacts", cards);
        }        
    }

    public class AuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }
}
