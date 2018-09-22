using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Controllers
{

    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class AdminController : BaseApiController
    {

        private readonly IAdminRepository _adminRepository;
        private readonly IAccountRepository _accountRepository;

        public AdminController(ICardRepository cardRepository, IAdminRepository adminRepository, IAccountRepository accountRepository)
        {
            _adminRepository = adminRepository;
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage UserList()
        {
            var users = _adminRepository.GetAllBusidexUsers();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Users = users.Select(u=>new
                    {
                        u.UserId, u.UserName, u.Email,
                        LastActivityDate = u.LastActivityDate.ToString(CultureInfo.InvariantCulture)
                    }).OrderByDescending(u=>u.UserId)
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SendCommunication([FromBody] AdminCommunication model)
        {
            //TODO: This could potentially be a long-running process so try it asynchronously. Might have to refactor this someday.
            bool error = false;
            var communication = new Communication();

            try
            {
                foreach (var email in model.SendTo)
                {
                    communication = new Communication
                    {
                        EmailTemplate = model.Template,
                        Email = email,
                        DateSent = DateTime.UtcNow,
                        UserId = model.UserId,
                        SentById = model.UserId,
                        OwnerToken = null,
                        Failed = false,
                        EmailTemplateId = model.Template.EmailTemplateId
                    };
                    SendEmail(communication);
                    _accountRepository.SaveCommunication(communication);
                }

            }
            catch (Exception ex)
            {
                error = true;
                _cardRepository.SaveApplicationError(ex, model.UserId);
                communication.Failed = true;

            }

            return await Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                var
                    response = error
                        ? new HttpResponseMessage
                        {
                            Content = new JsonContent(new
                            {
                                Success = false
                            }),
                            StatusCode = HttpStatusCode.InternalServerError
                        }
                        : new HttpResponseMessage
                        {
                            Content = new JsonContent(new
                            {
                                Success = true
                            }),
                            StatusCode = HttpStatusCode.OK
                        };
                return response;
            });
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage AdminCommunication(EmailTemplateCode code)
        {
            var template = _accountRepository.GetEmailTemplate(code);
            var users = _adminRepository.GetAllBusidexUsers();//.GetCardOwners();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Template = template,
                    Users = users.Select(u => new
                    {
                        u.UserId,
                        u.UserName,
                        u.Email,
                        LastActivityDate = u.LastActivityDate.ToString(CultureInfo.InvariantCulture)
                    }).OrderByDescending(u => u.UserId)
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage OwnerList()
        {
            var users = _adminRepository.GetCardOwners();
            
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Users = users.Where(u => u.CardFileId != Guid.Empty).Select(u => new
                    {
                        u.UserId, 
                        u.UserName, 
                        u.Email,
                        LastActivityDate = u.LastActivityDate.ToString(CultureInfo.InvariantCulture),
                        CardFileId = u.CardFileId.ToString() + "." + u.CardFileType
                    }).OrderByDescending(u => u.UserId)
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage ApplicationErrors(int daysBack)
        {
            var errors = _adminRepository.GetApplicationErrors(daysBack);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Errors = errors
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage NewCards(int daysBack)
        {
            var cards = _cardRepository.GetAllCards();
            var model = cards.Where(c => c.Updated > DateTime.UtcNow.AddDays(-daysBack)).OrderByDescending(c => c.Updated).ToList();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Cards = model
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage UnownedCards()
        {
            var cards = _adminRepository.GetAllUnownedCards();
            var model = cards.OrderByDescending(c => c.Updated).ToList();
               // .Where(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrEmpty(c.CompanyName) || string.IsNullOrEmpty(c.Email)||c.PhoneNumbers.Count==0)
                

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Cards = model
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage SendOwnerEmails(long userId, long cardId, string email)
        {
            userId = ValidateUser();
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

            var token = Guid.NewGuid();
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.ConfirmOwner);
            var card = _cardRepository.GetCardById(cardId, userId);
            if (card == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Card not found"
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            string height = card.FrontOrientation == "H" ? "120px" : "220px"; 
            string width = card.FrontOrientation == "H" ? "210px" : "140px";

            string backHeight = card.BackOrientation == "H" ? "120px" : "220px";
            string backWidth = card.BackOrientation == "H" ? "210px" : "140px";

            template.Body = template.Body.Replace("###", token.ToString());
            template.Body = template.Body.Replace("+++", card.FrontFileId + "." + card.FrontType);
            if (card.BackFileId != null && card.BackFileId.ToString().ToUpper() != "B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6")
            {
                template.Body = template.Body.Replace("---", card.BackFileId + "." + card.BackType);
                template.Body = template.Body.Replace("%DISPLAY_BACK%", "inline-block");
                template.Body = template.Body.Replace("%BHH%", backHeight);
                template.Body = template.Body.Replace("%BWW%", backWidth);
            }
            else
            {
                template.Body = template.Body.Replace("---", string.Empty);
                template.Body = template.Body.Replace("%BHH%", "0");
                template.Body = template.Body.Replace("%BWW%", "0");
                template.Body = template.Body.Replace("%DISPLAY_BACK%", "none");
            }
            template.Body = template.Body.Replace("%HH%", height);
            template.Body = template.Body.Replace("%WW%", width);

            bool error = false;

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                SentById = userId,
                OwnerToken = token,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            try
            {
                SendEmail(communication);
            }
            catch (Exception ex)
            {
                error = true;
                _cardRepository.SaveApplicationError(ex, userId);
                communication.Failed = true;

            }
            finally
            {
                _accountRepository.SaveCommunication(communication);
                _cardRepository.SaveCardOwnerToken(cardId, token);
            }

            return error
                ? new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                }
                : new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage SaveCardInfo(BasicCard card)
        {

            _cardRepository.UpdateCardBasicInfo(card.CardId, card.Name, card.CompanyName, card.PhoneNumber, card.Email);
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage PopularTags()
        {
            try
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Tags =
                            _adminRepository.GetPopularTags().Select(t => new {Tag = t.Key, Count = t.Value}).ToList()
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (SqlException ex)
            {
                if (!ex.Message.ToLower().Contains("transport-level error"))
                {
                    _cardRepository.SaveApplicationError(ex, 0);
                    
                }
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Tags = new Dictionary<string, int>()
                    }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Tags = new Dictionary<string, int>()
                    }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddSystemTag(string text)
        {
            _adminRepository.AddTag(text, (int) TagType.System);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage SystemTags()
        {
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Tags = _adminRepository.GetSystemTags()
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage UserMailingList()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(string.Join(Environment.NewLine, _adminRepository.GetAllBusidexUsers().Select(u => u.Email).ToArray())),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage DeviceSummary()
        {
            var summary = _adminRepository.GetDeviceSummary();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Summary = summary
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage DeviceDetails()
        {
            var details = _adminRepository.GetDeviceDetails();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Details = details
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Jobs(JobCode code)
        {

            switch (code)
            {
                case JobCode.CardUpdatedNotification:
                    {
                        _cardRepository.SendCardUpdatedEmails();
                        break;
                    }
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
