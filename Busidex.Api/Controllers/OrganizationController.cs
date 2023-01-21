using System.Collections.Generic;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class OrganizationController : BaseApiController
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly string cardUpdateStorageConnectionString;

        public OrganizationController(IOrganizationRepository organizationRepository, ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _organizationRepository = organizationRepository;
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            cardUpdateStorageConnectionString = ConfigurationManager.AppSettings["BusidexQueuesConnectionString"];
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {
            var userId = ValidateUser();
            if (userId == 0)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            var model = _organizationRepository.GetOrganizationsByUserId(userId).OrderByDescending(o=>o.IsMember).ThenBy(o=>o.Name).ToList();           

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Model = model ?? new List<Organization>()
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long id)
        {
            var userId = ValidateUser();
            if (userId == 0)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            var model = _organizationRepository.GetOrganizationById(id) ?? new Organization();
            model.LogoFilePath = ConfigurationManager.AppSettings["userCardPath"];

            var groups = _organizationRepository.GetOrganizationGroups(id, userId);
            var members = _organizationRepository.GetOrganizationCards(id, userId);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    IsAdmin = model.UserId == userId,
                    OrgRole = model.UserId == userId ? "Admin" : members.Any(m=> m.OwnerId == userId) ? "Member" : "Guest",
                    Model = model,
                    Groups = groups
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        //[System.Web.Http.HttpGet]
        //public HttpResponseMessage GetOrganizationById(long? id = null)
        //{
        //    var userId = ValidateUser();
        //    if (userId == 0)
        //    {
        //        return new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.Unauthorized
        //        };
        //    }

        //    var orgId = id.GetValueOrDefault();
        //    var model = _organizationRepository.GetOrganizationById(orgId) ?? new Organization();
        //    model.LogoFilePath = ConfigurationManager.AppSettings["userCardPath"];

        //    var groups = _organizationRepository.GetOrganizationGroups(orgId, userId);
        //    var members = _organizationRepository.GetOrganizationCards(orgId, userId);

        //    return new HttpResponseMessage
        //    {
        //        Content = new JsonContent(new
        //        {
        //            Success = true,
        //            IsAdmin = model.UserId == userId,
        //            OrgRole = model.UserId == userId ? "Admin" : members.Any(m => m.OwnerId == userId) ? "Member" : "Guest",
        //            Model = model,
        //            Groups = groups
        //        }),
        //        StatusCode = HttpStatusCode.OK
        //    };
        //}

        [System.Web.Http.HttpPost]
        public HttpResponseMessage UpdateLogo(long id)
        {
            var userId = ValidateUser();

            var organization = _organizationRepository.GetOrganizationById(id);
            if (organization == null || userId == 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var logo = HttpContext.Current.Request.Files.Count > 0
                    ? HttpContext.Current.Request.Files[0]
                    : null;
            if (logo != null && logo.ContentLength > 0)
            {
                var logoBuffer = new byte[logo.InputStream.Length];
                logo.InputStream.Read(logoBuffer, 0, (int)logo.InputStream.Length);

                var extension = Path.GetExtension(logo.FileName);
                organization.LogoType = extension.ToLower().Replace(".", "").Trim();
                organization.Logo = logoBuffer;

                _organizationRepository.LogoToFile(organization);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.NotModified
            };
        }

        public HttpResponseMessage Post([FromBody] Organization organization)
        {
            var userId = ValidateUser();

            var testOrg = _organizationRepository.GetOrganizationsByUserId(userId);
            if (testOrg.Count == 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                }; 
            }

            var existingAccount = _accountRepository.GetUserAccountByEmail(organization.Email);
            if (existingAccount != null && existingAccount.BusidexUser.Email != organization.Email)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Cannot use this email address"
                };
            }

            _organizationRepository.AddOrganization(organization, userId);            

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
        public HttpResponseMessage Update([FromBody] Organization organization)
        {
            var userId = ValidateUser();
            string cardMessage = "";
            try
            {
                if (organization == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            ReasonPhrase = "No payload found"
                        }),
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "No payload found"
                    };
                }

                var testOrg = _organizationRepository.GetOrganizationById(organization.OrganizationId);

                if (testOrg == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            ReasonPhrase = "Organization not found"
                        }),
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "Organization not found"
                    };
                }

                //if (!string.IsNullOrEmpty(organization.Email))
                //{
                //    var existingAccount = _accountRepository.GetUserAccountByEmail(organization.Email);
                //    if (existingAccount != null && existingAccount.BusidexUser.Email != organization.Email)
                //    {
                //        return new HttpResponseMessage
                //        {
                //            Content = new JsonContent(new
                //            {
                //                Success = false,
                //                Message = "Cannot use this email address"
                //            }),
                //            StatusCode = HttpStatusCode.BadRequest,
                //            ReasonPhrase = "Cannot use this email address"
                //        };
                //    }
                //}

                // Only allow the admin to edit
                if (testOrg.UserId != userId)
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
                

                _organizationRepository.UpdateOrganization(organization, userId);
                cardMessage += "organization updated";

                var orgCard = _cardRepository.GetOrganizationCardByOwnerId(organization.OrganizationId);
                cardMessage += " | got organization by ownerId";
                if (orgCard != null)
                {
                    var model = new AddOrEditCardModel(orgCard)
                    {
                        Visibility = (CardVisibility)organization.Visibility,
                        Action = AddOrEditCardModel.CardAction.Edit
                    };

                    cardMessage += " | new model created with visibility = " + model.Visibility;
                    var cardRef = Guid.NewGuid().ToString();
                    _cardRepository.UploadCardUpdateToBlobStorage(model, cardUpdateStorageConnectionString, cardRef);
                    _cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
                    cardMessage += " | card added to queue";
                }
                else
                {
                    cardMessage = "Couldn't update card visibility";
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        ExtraData = cardMessage
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
                        ExtraData = cardMessage
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetGuests(long organizationId)
        {
            var userId = ValidateUser();
            if (userId == 0)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };      
            }

            var model = _organizationRepository.GetOrganizationGuests(organizationId, userId);
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Guests = model
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetMembers(long organizationId)
        {
            var userId = ValidateUser();

            var model = _organizationRepository.GetOrganizationCards(organizationId, userId);
            var sorted = new List<CardDetailModel>();
            sorted.AddRange(model.Where(c => c.OwnerId == userId));
            sorted.AddRange(
                model
                .Where(c => c.OwnerId != userId)
                    .OrderByDescending(c => c.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                    .ThenBy(c => c.CompanyName)
                    .ThenBy(c => c.Name)
                    .ToList());
            //var myBusidex = _cardRepository.GetMyBusidex(userId, false);
            //model.ForEach(c => c.ExistsInMyBusidex = myBusidex.Any(b => b.CardId == c.CardId));

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Model = sorted

                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetReferrals(long? organizationId)
        {
            if(organizationId.GetValueOrDefault() == 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = new List<UserCard>()
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            var userId = ValidateUser();
            try {
                var model = _organizationRepository.GetOrganizationReferrals(userId, organizationId.Value);
                var sorted = new List<UserCard>();
                sorted.AddRange(model.Where(c => c.Card.OwnerId == userId));
                sorted.AddRange(
                    model
                    .Where(c => c.Card.OwnerId != userId)
                        .OrderByDescending(c => c.Card.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Card.Name)
                        .ThenBy(c => c.Card.CompanyName)
                        .ToList());
                //var myBusidex = _cardRepository.GetMyBusidex(userId, false);
                //model.ForEach(c => c.Card.ExistsInMyBusidex = myBusidex.Any(b => b.CardId == c.CardId));

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = sorted
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }catch(Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = new List<UserCard>()
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // NOT USED
        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddOrganaizationCard(long organizationId, long cardId)
        {
            var userId = ValidateUser();

            _organizationRepository.AddOrganizationCard(organizationId, cardId, userId);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage AddOrganizationCard(long organizationId, long cardId)
        {
            var userId = ValidateUser();

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

            _organizationRepository.AddOrganizationCard(organizationId, cardId, userId);

            // Remove the card from the referrals
            var uc = _cardRepository.GetUserCard(cardId, userId);
            if (uc != null)
            {
                _cardRepository.DeleteUserCard(uc, userId);
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

        // NOT USED
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteOrganaizationCard(long organizationId, long cardId)
        {
            var userId = ValidateUser();

            _organizationRepository.DeleteOrganizationCard(organizationId, cardId, userId);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteOrganizationCard(long organizationId, long cardId)
        {
            var userId = ValidateUser();

            _organizationRepository.DeleteOrganizationCard(organizationId, cardId, userId);            

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        //[System.Web.Http.HttpGet]
        //public HttpResponseMessage Testing()
        //{
        //    try
        //    {
        //        var testOrg = _cardRepository.GetOrganizationCardByOwnerId(11);
        //        return new HttpResponseMessage
        //        {
        //            Content = new JsonContent(new
        //            {
        //                Success = true
        //            }),
        //            StatusCode = HttpStatusCode.OK
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new HttpResponseMessage
        //        {
        //            Content = new JsonContent(new
        //            {
        //                Success = false,
        //                Extra = ex
        //            }),
        //            StatusCode = HttpStatusCode.OK
        //        };
        //    }
            
        //}
    }
}
