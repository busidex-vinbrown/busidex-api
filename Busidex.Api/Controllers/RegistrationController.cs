using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
//using System.Web.Http.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;
using Twilio;
using BusidexUser = Busidex.Api.DataAccess.DTO.BusidexUser;
using UserAccount = Busidex.Api.DataAccess.DTO.UserAccount;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class RegistrationController : BaseApiController
    {
        const int ACCOUNT_TYPE_ORGANIZATION = 7;
        private readonly IAccountRepository _accountRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public RegistrationController(IAccountRepository accountRepository, ICardRepository cardRepository, IAdminRepository adminRepository, IOrganizationRepository organizationRepository)
        {
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _adminRepository = adminRepository;
            _organizationRepository = organizationRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(string id = "")
        {

            try
            {
                long sharedById;
                long.TryParse(id, out sharedById);
                CardDetailModel card = null;
                if (!string.IsNullOrEmpty(id))
                {
                    card = sharedById == 0
                        ? _cardRepository.GetCardByToken(id)
                        : _cardRepository.GetCardsByOwnerId(sharedById).FirstOrDefault();
                }
                var plans = _accountRepository.GetActivePlans().OrderBy(p => p.DisplayOrder).ToList();

                return new HttpResponseMessage
                       {
                           Content = new JsonContent(new
                           {
                               Plans = plans,
                               Card = card,
                               InviteUserId = sharedById
                           })
                       };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "error" })
                };
            }
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage ActivateAccount(Guid? token = null)
        {
            var userId = ValidateUser();
            if (userId == 0)
            {
                var ua =_accountRepository.GetUserAccountByToken(token.GetValueOrDefault());
                if (ua == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new {Success = false}),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }
                userId = ua.UserId;
            }

            _accountRepository.ActivateUserAccount(userId);

            var buser = _accountRepository.GetBusidexUserById(userId);
            var cards = _cardRepository.GetCardsByOwnerId(userId);
            var organizations = _organizationRepository.GetOrganizationsByUserId(userId);
            var organizationMenuItems = organizations.Select(o => new Tuple<string, long>(o.Name, o.OrganizationId)).ToList();
            var tokenBytes = Encoding.ASCII.GetBytes(buser.UserId.ToString(CultureInfo.InvariantCulture));
            var encodedUserId = Convert.ToBase64String(tokenBytes);

            return new HttpResponseMessage
            {
                Content = new JsonContent(
                    new User
                    {
                        UserId = buser.UserId,
                        UserName = buser.UserName,
                        IsAdmin = Roles.IsUserInRole(buser.UserName, "Administrator"),
                        HasCard = cards.Count > 0,
                        UserAccountId = buser.UserAccount.UserAccountId,
                        AccountTypeId = buser.UserAccount.AccountTypeId,
                        CardId = cards.Count > 0 ? cards[0].CardId : 0,
                        Token = encodedUserId,
                        ActivationToken = buser.UserAccount.ActivationToken,
                        StartPage = (buser.UserAccount.AccountTypeId == ACCOUNT_TYPE_ORGANIZATION && organizationMenuItems.Count > 0) ? "Organization" : "Index",
                        Organizations = organizationMenuItems,
                        DisplayName = !string.IsNullOrEmpty(buser.UserAccount.DisplayName) ? buser.UserAccount.DisplayName : buser.UserName
                    }),
                StatusCode = HttpStatusCode.OK
            };
           
        }


        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put(string token)
        {
            
            long userId = 0;
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new NullReferenceException("Invalid registration token");
                }

                UserAccount userAccount;
                const int GUID_LENGTH = 36;

                if (token.Length == GUID_LENGTH)
                {
                    var tokenGuid = new Guid(token);
                    userAccount = _accountRepository.GetUserAccountByToken(tokenGuid);
                }
                else
                {
                    userAccount = _accountRepository.GetUserAccountByCode(token);
                }

                if (userAccount == null)
                {
                    throw new InvalidDataException("Registration token not found");
                }
               
                var model = new UserAccountModel
                {
                    UserAccount = userAccount,
                    Email = string.Empty
                };

                MembershipUser user = Membership.GetUser(userAccount.UserId);

                if (user != null)
                {
                    model.Email = user.Email;

                    if (!userAccount.Active)
                    {
                        _accountRepository.ActivateUserAccount(token);
                        
                    }
                    SendWelcomeEmail(userId, user.Email);
                    userId = user.ProviderUserKey != null ? (long)user.ProviderUserKey : 0;

                    var buser = _accountRepository.GetBusidexUserById(userId);
                    var cards = _cardRepository.GetCardsByOwnerId(userId);
                    var organizations = _organizationRepository.GetOrganizationsByUserId(userId);
                    var organizationMenuItems = organizations.Select(o => new Tuple<string, long>(o.Name, o.OrganizationId)).ToList();
                    var tokenBytes = Encoding.ASCII.GetBytes(buser.UserId.ToString(CultureInfo.InvariantCulture));
                    var encodedUserId = Convert.ToBase64String(tokenBytes);

                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(
                            new User
                            {
                                UserId = buser.UserId,
                                UserName = buser.UserName,
                                IsAdmin = Roles.IsUserInRole(buser.UserName, "Administrator"),
                                HasCard = cards.Count > 0,
                                UserAccountId = buser.UserAccount.UserAccountId,
                                AccountTypeId = buser.UserAccount.AccountTypeId,
                                CardId = cards.Count > 0 ? cards[0].CardId : 0,
                                Token = encodedUserId,
                                StartPage = (buser.UserAccount.AccountTypeId == ACCOUNT_TYPE_ORGANIZATION && organizationMenuItems.Count > 0) ? "Organization" : "Index",
                                Organizations = organizationMenuItems,
                                DisplayName = !string.IsNullOrEmpty(buser.UserAccount.DisplayName) ? buser.UserAccount.DisplayName : buser.UserName
                            }),
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "Account not active" }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            catch (NullReferenceException nEx)
            {
                _cardRepository.SaveApplicationError(nEx, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new {nEx.Message}),
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            catch (InvalidDataException iEx)
            {
                _cardRepository.SaveApplicationError(iEx, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new {iEx.Message}),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new {Message = "error"}),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public class AutoResponseForm
        {
            public string uidId { get; set; }
            public string email { get; set; }
            public string pswd { get; set; }
            //public bool? acceptTerms { get; set;  }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CheckAccount([FromBody] AutoResponseForm form)
        {
            try
            {
                string uidId = form.uidId;
                string email = form.email;
                string password = form.pswd;
                //bool? acceptTerms = form.acceptTerms;

                //if (acceptTerms.HasValue && !acceptTerms.GetValueOrDefault())
                //{
                //    return new HttpResponseMessage
                //    {
                //        Content = new JsonContent(new
                //        {
                //            Success = false,
                //            UserId = -1
                //        }),
                //        StatusCode = HttpStatusCode.BadRequest,
                //        ReasonPhrase = "Terms not accepted"
                //    }; 
                //}

                if (string.IsNullOrEmpty(uidId) || string.IsNullOrEmpty(email))
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            UserId = -1
                        }),
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "No unique identifier"
                    };
                }

                string userName = email.Substring(0, email.IndexOf("@", StringComparison.Ordinal));

                var user = _accountRepository.GetUserAccountByEmail(email);
                if (user == null)
                {
                    MembershipCreateStatus createStatus;
                    MembershipUser newMobileUser = Membership.CreateUser(userName, password, email, null, null, true,
                        null,
                        out createStatus);
                    if (newMobileUser != null)
                    {
                        var token = Guid.NewGuid(); // activate automatically
                        var userId = (long) newMobileUser.ProviderUserKey;

                        var newUserAccount = new UserAccount
                        {
                            AccountTypeId = 6,
                            UserId = userId,
                            Created = DateTime.Now,
                            Active = true,
                            BusidexUser = _accountRepository.GetBusidexUserById((long)newMobileUser.ProviderUserKey),
                            ActivationToken = token
                        };
                        _accountRepository.AddUserAccount(newUserAccount);
                        _accountRepository.ActivateUserAccount(token.ToString());

                        _accountRepository.AcceptUserTerms(newUserAccount.UserId);

                        //If this person does not have a card, give them a stub record 
                        if (_cardRepository.GetCardsByOwnerId(userId).Count == 0)
                        {
                            long cardId;
                            var card = new Card
                            {
                                FrontFileId = Guid.Empty,
                                BackFileId = Guid.Empty
                            };
                            _cardRepository.AddCard(card, true, userId, "", out cardId);
                        }

                        
                        return new HttpResponseMessage
                        {
                            Content = new JsonContent(new
                            {
                                Success = true,
                                UserId = userId,
                                ReasonPhrase = string.Empty
                            }),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            UserId = -1,
                            ReasonPhrase = "Unable to create new account: " + createStatus
                        }),
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "Unable to create new account: " + createStatus
                    };
                }

                // Account alread exists. Return the userId.
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        UserId = -1,
                        ReasonPhrase = "Account already exists"
                    }),
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Account already exists"
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, -1);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        UserId = -1,
                        ReasonPhrase = "Account already exists"
                    }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }

        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post()
        {
            long userId = 0;

            try
            {
                var s = new JavaScriptSerializer();
                var jsmodel = s.Deserialize<RegisterModel>(HttpContext.Current.Request.Form["model"]);
                jsmodel.Password = HttpUtility.UrlDecode(jsmodel.Password);
                jsmodel.ConfirmPassword = HttpUtility.UrlDecode(jsmodel.ConfirmPassword);
                jsmodel.Password = HttpUtility.HtmlDecode(jsmodel.Password);
                jsmodel.ConfirmPassword = HttpUtility.HtmlDecode(jsmodel.ConfirmPassword);

                var ua = s.Deserialize<UserAccount>(HttpContext.Current.Request.Form["model"]);

                if (jsmodel.IsMobile && string.IsNullOrEmpty(jsmodel.MobileNumber))
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new { Message = "error" }),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                // Validate Promo Code
                if (!string.IsNullOrEmpty(jsmodel.PromoCode))
                {
                    var tag = _adminRepository.GetTag(TagType.System, jsmodel.PromoCode);
                    if (tag == null)
                    {
                        return new HttpResponseMessage
                        {
                            Content = new JsonContent(new { Message = "The Pomo Code you entered wasn not found." }),
                            StatusCode = HttpStatusCode.NotFound
                        };
                    }
                }

                MembershipCreateStatus createStatus;
                MembershipUser u = Membership.CreateUser(jsmodel.DisplayName, jsmodel.Password, jsmodel.Email, null, null, true, null, out createStatus);

                if (createStatus != MembershipCreateStatus.Success)
                {
                    throw new MembershipCreateUserException(createStatus);
                }

                if (u == null || u.ProviderUserKey == null)
                {
                    _cardRepository.SaveApplicationError(
                        new NullReferenceException("Registration error. Couldn't create MembershipUser for some reason."),
                        0);
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new {Message = "error"}),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                BusidexUser bu = _accountRepository.GetBusidexUserById((long) u.ProviderUserKey);

                userId = ua.UserId = bu.UserId;

                _accountRepository.AcceptUserTerms(userId);

                var activationToken = Guid.NewGuid();

                var newUserAccount = _accountRepository.AddUserAccount(ua);
                ProcessPromoCode(newUserAccount, jsmodel.PromoCode);

                if (jsmodel.IsMobile)
                {
                    SendSMS(userId, jsmodel.MobileNumber);
                }
                else
                {
                    if (jsmodel.InviteUserId > 0)
                    {
                        // this person was invited by a card owner or organization 
                        var sharedCard = _cardRepository.GetCardsByOwnerId(jsmodel.InviteUserId).FirstOrDefault();
                        if (sharedCard != null)
                        {
                            _cardRepository.AddToMyBusidex(sharedCard.CardId, bu.UserId);

                            var eventSources = _cardRepository.GetAllEventSources();
                            var eventSourceId =
                                eventSources.Single(src => src.EventCode == EventSources.SHARE.ToString()).EventSourceId;

                            _cardRepository.AddEventActivity(new EventActivity
                            {
                                CardId = sharedCard.CardId,
                                UserId = userId,
                                ActivityDate = DateTime.UtcNow,
                                EventSourceId = eventSourceId
                            });
                        }

                    } else if (!string.IsNullOrEmpty(jsmodel.CardOwnerToken))
                    {
                        // an Admin sent this person an invite token, so they are the card owner
                        CardDetailModel card = _cardRepository.GetCardByToken(jsmodel.CardOwnerToken);
                        if (card != null)
                        {
                            _cardRepository.SaveCardOwner(card.CardId, bu.UserId);
                            _cardRepository.AddToMyBusidex(card.CardId, bu.UserId);
                            _cardRepository.AddSendersCardToMyBusidex(jsmodel.CardOwnerToken, bu.UserId);
                        }
                    }else if (jsmodel.InviteCardToken.HasValue)
                    {
                        var card = _cardRepository.GetCardByToken(jsmodel.InviteCardToken.GetValueOrDefault().ToString());
                        if (card != null)
                        {
                            var myBusidex = _cardRepository.GetMyBusidex(userId, false);
                            // make sure this user doesn't already have the card
                            if (myBusidex.All(c => c.CardId != card.CardId))
                            {
                                _cardRepository.AddToMyBusidex(card.CardId, userId);
                            }
                        }
                    }

                    //If this person does not have a card, give them a stub record 
                    if (_cardRepository.GetCardsByOwnerId(bu.UserId).Count == 0)
                    {
                        long cardId;
                        _cardRepository.AddCard(new Card(), true, bu.UserId, "", out cardId);

                        // Special case for Minute Man Press users. Add the MM Press card to their Busidex collection
                        if (jsmodel.PromoCode == "MMPress")
                        {
                            const long MINUTEMAN_PRESS_CARD_ID = 102199;
                            _cardRepository.AddToMyBusidex(MINUTEMAN_PRESS_CARD_ID, bu.UserId);
                        }
                    }

                    
                    _accountRepository.SaveUserAccountToken(userId, activationToken);

                    SendConfirmationEmail(bu.UserId, jsmodel.Email, activationToken);
                }
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "success", Token = activationToken }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (MembershipCreateUserException mEx)
            {
                //_cardRepository.SaveApplicationError(mEx, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = ErrorCodeToString(mEx.StatusCode) }),
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = mEx.Message
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "There was a problem processing your registration." }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage TestSMS(long userId, string number)
        {
            try
            {
                SendSMS(userId, number);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new {Success = true})
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Success = false, Error = ex.Message })
                };
            }
        }

        private void ProcessPromoCode(UserAccount ua, string code)
        {
            if (ua == null || string.IsNullOrEmpty(code))
            {
                return;
            }

            var tag = _adminRepository.GetSystemTags().SingleOrDefault(t => t.Text == code);
            if (tag != null)
            {
                _accountRepository.AddUserAccountTag(ua.UserAccountId, tag.TagId);
            }
        }

        private void SendWelcomeEmail(long userId, string email)
        {
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Welcome);

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = userId,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            try
            {
                SendEmail(communication);
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                communication.Failed = true;
            }
            finally
            {
                _accountRepository.SaveCommunication(communication);
            }
        }

        private void SendConfirmationEmail(long userId, string email, Guid token)
        {            
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Registration);

            template.Body = template.Body.Replace("###", token.ToString());

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = userId,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            try
            {
                SendEmail(communication);
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                communication.Failed = true;
            }
            finally
            {
                _accountRepository.SaveCommunication(communication);
            }
        }

        private void SendSMS(long userId, string number)
        {
            try
            {
                
                string sId = ConfigurationManager.AppSettings["SMSsId"];
                string authToken = ConfigurationManager.AppSettings["SMSAuthToken"];
                string busidexNumber = ConfigurationManager.AppSettings["SMSNumber"];
                int codeLength = int.Parse(ConfigurationManager.AppSettings["SMSCodeLength"]);

                // Create an instance of the Twilio client.
                var client = new TwilioRestClient(sId, authToken);

                // Generate the confirmation code
                string code = GenerateNewCode(codeLength);

                _accountRepository.SaveUserAccountCode(userId, code);

                // Send SMS message with registration code.
                SMSMessage result = client.SendSmsMessage(
                    busidexNumber, 
                    number,
                    "Use this code to complete your Busidex Registration: " + code);

                if (result.RestException != null)
                {
                    //an exception occurred making the REST call
                    _cardRepository.SaveApplicationError(new Exception(result.RestException.Message), userId);
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
            }
            
        }

        public static string GenerateNewCode(int len)
        {
            var random = new Random(DateTime.Now.Millisecond);
            
                var output = new StringBuilder();

                for (int i = 0; i < len; i++)
                {
                    output.Append(random.Next(0, 9));
                }
            return output.ToString();
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
