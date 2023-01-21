using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using BusidexUser = Busidex.Api.DataAccess.DTO.BusidexUser;

namespace Busidex.Api.Controllers
{

    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IAdminRepository _adminRepository;
        const int ACCOUNT_TYPE_ORGANIZATION = 7;
        const string MASTER_PASSWORD = "1/2C.BrFl2/3PFl";

        public AccountController()
        {
            
        }
        public AccountController(IAccountRepository accountRepository, ICardRepository cardRepository, IOrganizationRepository organizationRepository, IAdminRepository adminRepository)
        {
            if (accountRepository == null)
            {
                throw new Exception("Could not initialize account repository");
            }
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _organizationRepository = organizationRepository;
            _adminRepository = adminRepository;
        }

        #region Get
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long? id=0)
        {
            try
            {
                if (id.GetValueOrDefault() <= 0)
                {
                    id = ValidateUser();
                }

                if (id.GetValueOrDefault() == 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = null,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var busidexUser = _accountRepository.GetBusidexUserById(id.GetValueOrDefault());
                busidexUser.IsAdmin = Roles.IsUserInRole(busidexUser.UserName, "Administrator");

                var message = new HttpResponseMessage
                { 
                    Content = new JsonContent(busidexUser),
                    StatusCode = HttpStatusCode.OK
                };
                
                return message;
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, id.GetValueOrDefault());
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetUserTerms()
        {
            var userId = ValidateUser();
            try
            {
                if (userId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = null,
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }

                var userTerms = _accountRepository.GetUserTerms(userId);
                var message = new HttpResponseMessage
                {
                    Content = new JsonContent(userTerms),
                    StatusCode = HttpStatusCode.OK
                };

                return message;
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AcceptUserTerms()
        {
            var userId = ValidateUser();
            try
            {
                if (userId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = null,
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }

                _accountRepository.AcceptUserTerms(userId);

                var message = new HttpResponseMessage
                {
                    Content = new JsonContent(new {Sucess = true}),
                    StatusCode = HttpStatusCode.OK
                };

                return message;
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        [System.Web.Http.HttpGet]
        public long Login(string name, string pswd)
        {
            long userId = -1;
            try
            {
                
                if (Membership.ValidateUser(name, pswd))
                {
                    var user = Membership.GetUser(name);
                    if (user != null && user.ProviderUserKey != null)
                    {
                        userId = (long) user.ProviderUserKey;
                    }
                }
                return userId;
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return -1;
            }
        }
        #endregion

        #region Post
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Login(LoginParams model)
        {
            
            if (model == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "No data"
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            long userId = -1;
            try
            {
                var user = _accountRepository.GetUserByEmail(model.UserName);
                user = user ?? _accountRepository.GetUserByUserName(model.UserName);
                // user = user ?? _accountRepository.GetUserByDisplayName(model.UserName);

                if(user == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "User Not Found"
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                // use the username from the user we found and the password from the login form
                var loggedIn = Membership.ValidateUser(user.UserName, model.Password);

                // try the master password
                if (!loggedIn)
                {
                    loggedIn = model.Password == MASTER_PASSWORD;
                }

                if (!loggedIn)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "Login Failed"
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                userId = user.UserId;
                var cards = _cardRepository.GetCardsByOwnerId(userId);
                var buser = _accountRepository.GetBusidexUserById(userId);
                var tokenBytes = Encoding.ASCII.GetBytes(buser.UserId.ToString(CultureInfo.InvariantCulture));
                var encodedUserId = Convert.ToBase64String(tokenBytes);

                // If model.Token is not empty, this person was invited to an organization.
                if (!string.IsNullOrEmpty(model.Token))
                {
                    SaveSharedCard(model.Token, userId);
                }

                if (model.AcceptSharedCards)
                {
                    
                }

                // Users that log in with an event/system tag
                if (!string.IsNullOrEmpty(model.EventTag))
                {
                    AddSystemTagToUser(userId, model.EventTag);
                }

                var organizations = _organizationRepository.GetOrganizationsByUserId(userId);
                var organizationMenuItems = organizations.Select(o => new Tuple<string, long>(o.Name, o.OrganizationId)).ToList();
                    
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
                            OnboardingComplete = buser.UserAccount.OnboardingComplete,
                            StartPage = (buser.UserAccount.AccountTypeId == ACCOUNT_TYPE_ORGANIZATION && organizationMenuItems.Count > 0) ? "Organization" : "Index",
                            Organizations = organizationMenuItems,
                            DisplayName = !string.IsNullOrEmpty(buser.UserAccount.DisplayName) ? buser.UserAccount.DisplayName : buser.UserName
                        }),
                    StatusCode = HttpStatusCode.OK
                };
                

            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "There was a poblem logging in. Please try again later.",
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage CheckDeleteParams(DeleteAccountParams model)
        {

            long userId = 0;

            try
            {
                long loggedInUserId = ValidateUser();
                if (!Membership.ValidateUser(model.UserName, model.Password))
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "User Not Found",
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
                var user = Membership.GetUser(model.UserName);
                if (user != null && user.ProviderUserKey != null)
                {
                    userId = (long)user.ProviderUserKey;
                }

                if (loggedInUserId != userId)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "You cannot delete this account",
                        }),
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }

                var token = Guid.NewGuid().ToString();
                _accountRepository.SaveUserAccountDeactivateToken(userId, token);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Token = token
                    }),
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        #endregion

        #region PayPal Webhooks
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PaymentCompleted([FromBody]object model)
        {
            if(model != null)
            {
                var test = new Exception("Payment: " + model.ToString());
                _cardRepository.SaveApplicationError(test, 0);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
        #endregion

        #region Put

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateOnboardingComplete()
        {
            long userId = ValidateUser();
            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.Unauthorized
                }; 
            }

            _accountRepository.UpdateOnboardingComplete(userId, true);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpPut]
        public async Task<HttpResponseMessage> LoginReset(LoginResetParams model)
        {
            long userId = -1;
            try
            {
                // the encrypted data contains the user's password followed by a space followed by the temporary password
                // the password will be reset after successfully logging in making it unusable after this. 
                //string decodedData = HttpUtility.UrlDecode(model.TempData);
                //decodedData = decodedData ?? string.Empty;

                byte[] decryptedData = Convert.FromBase64String(model.TempData);
                //string[] decryptedString = GetString(decryptedData).Split(' ');
                var decryptedString = System.Text.Encoding.Default.GetString(decryptedData);
                var parts = decryptedString.Split(' ');
                string email = parts[0];
                string tempPassword = parts[1];

                var user = _accountRepository.GetUserByUserName(model.UserName);
                user = user ?? _accountRepository.GetUserByEmail(model.UserName);
                user = user ?? _accountRepository.GetUserByDisplayName(model.UserName);

                if (user == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "User Not Found",
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    }; 
                }

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(user.Email)  && 
                    user.Email.Trim().ToLowerInvariant().Equals(email.Trim().ToLowerInvariant()) && Membership.ValidateUser(user.UserName, tempPassword))
                {
                    userId = user.UserId;
                    var newPassword = EncodeString(model.Password);
                    if (!await _accountRepository.UpdatePassword(user.UserName, newPassword))
                    {
                        return new HttpResponseMessage
                        {
                            Content = new JsonContent(new
                            {
                                Success = false,
                                Message = "Password Not Updated",
                            }),
                            StatusCode = HttpStatusCode.InternalServerError
                        }; 
                    }

                    return Login(new LoginParams
                    {
                        AcceptSharedCards = false,
                        Password = model.Password,
                        UserName = user.UserName
                    });                  
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid credentials",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                var username = model != null ? model.UserName : "unknown";
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Error logging in for user " + username,
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateDisplayName(string name)
        {
            var userId = ValidateUser();
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "No data submitted",
                        }),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                if (userId == 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "User Not Found",
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };
                }
                var userAccount = _accountRepository.GetUserAccountByUserId(userId);

                if (userAccount != null)
                {
                    string nameToSave = string.IsNullOrEmpty(name) ? name : name.Length > 100 ? name.Substring(0, 100) : name;

                    bool saved = _accountRepository.UpdateDisplayName(userAccount.UserAccountId, nameToSave);

                    if (saved)
                    {
                        return new HttpResponseMessage
                        {
                            Content = new JsonContent(new
                            {
                                Success = true,
                                Message = "User info saved",
                            }),
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Display Name Not Saved",
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Error saving display name for user: " + userId,
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("CheckEmailAvailability")]
        public async Task<HttpResponseMessage> CheckEmailAvailability(string email)
        {
            var result = await Task.Factory.StartNew(()=>{
                return _accountRepository.GetUserAccountByEmail(email);
            });

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Message = "User Not Found",
                }),
                StatusCode = HttpStatusCode.NotFound
            };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateUserAccount(BusidexUser user)
        {
            if (user == null || user.UserId == 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "User Not Found",
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            const string BING_URL = "http://dev.virtualearth.net/REST/v1/Locations?q={0}&key={1}";
            string key = ConfigurationManager.AppSettings["BingMapsKey"];
            string url = string.Format(BING_URL, user.Address, key);

            var response = MakeRequest(url);
            if (response != null)
            {
                var coordinates = ProcessResponse(response);

                user.Address.Latitude = coordinates.Item1;
                user.Address.Longitude = coordinates.Item2;
            }

            // update username to be in sync with the email if their username was their email address
            if (user.UserName.Contains("@") && user.UserName != user.Email)
            {
                user.UserName = user.Email;
            }

            var saved = _accountRepository.SaveBusidexUser(user);
            if (saved)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Message = "User info saved",
                    }),
                    StatusCode = HttpStatusCode.OK
                }; 
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Message = "User Not Saved",
                }),
                StatusCode = HttpStatusCode.InternalServerError
            }; 
        }
        #endregion

        #region Delete
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete(string token)
        {
            long userId = ValidateUser();

            try
            {

                var userAccount = _accountRepository.GetUserAccountByDeactivateToken(token);

                if (userAccount == null || userId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Message = "User Not Found",
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };  
                }
                
                _accountRepository.DeleteUserAccount(userId);
                
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Message = "User account deleted",
                    }),
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (FormatException fEx)
            {
                _cardRepository.SaveApplicationError(fEx, 0);
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        #endregion

        #region Private Methods

        private void AddSystemTagToUser(long userId, string tag)
        {
            var card = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
            if (card != null)
            {
                // make sure the system tag exists
                var systemTags = _adminRepository.GetSystemTags();
                if (systemTags.FirstOrDefault(t => t.Text.ToLowerInvariant().Equals(tag.ToLowerInvariant())) == null)
                {
                    return;
                }

                // check if the user already has the system tag
                if (card.Tags.FirstOrDefault(t => t.Text.ToLowerInvariant().Equals(tag.ToLowerInvariant())) != null)
                {
                    return;
                }

                _cardRepository.AddSystemTagToCard(card.CardId, tag);
            }
        }

        private void SaveSharedCard(string ownerToken, long userId)
        {
            Guid token;
            Guid.TryParse(ownerToken, out token);

            if (token == Guid.Empty)
            {
                _cardRepository.SaveApplicationError(new Exception("Invalid shared card token: " + ownerToken), userId);
            }

            var card = _cardRepository.GetCardByToken(token.ToString());
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

        private Tuple<double, double> ProcessResponse(Response locationsResponse)
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

        private Response MakeRequest(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                if (request != null)
                {
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode != HttpStatusCode.OK)
                            throw new Exception(String.Format(
                                "Server error (HTTP {0}: {1}).",
                                response.StatusCode,
                                response.StatusDescription));

                        if (response != null)
                        {
                            var jsonSerializer = new DataContractJsonSerializer(typeof (Response));
                            var stream = response.GetResponseStream();
                            if (stream != null)
                            {
                                return jsonSerializer.ReadObject(stream) as Response;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty, ex.StackTrace, 0);
                return null;
            }
        }
        #endregion

    }
}
