using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class BusidexController : BaseApiController
    {
        public BusidexController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long id = 0, bool all = true)
        {
            
            try
            {
                if (id == 0)
                {
                    id = ValidateUser();
                }

                List<UserCard> list = _cardRepository.GetMyBusidex(id, false).Where(c => c.Card != null).ToList();
                var sorted = new List<UserCard>();
                sorted.AddRange(list.Where(c => c.Card.OwnerId == id));
                sorted.AddRange(
                    list
                    .Where(c => c.Card.OwnerId != id && (all || c.MobileView))
                        .OrderByDescending(c => c.Card.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Card.Name)
                        .ThenBy(c => c.Card.CompanyName)
                        .ToList());

                //var allCards = sorted.Select(userCard => userCard.Card).ToList();
                List<string> allTags = (from cards in sorted.Select(s=>s.Card)
                    from tag in cards.Tags
                    select tag.Text).ToList();

                var tags = (from tag in allTags
                    group tag by tag
                    into t
                    select new {key = t.First(), Value = t.Count()})
                    .ToDictionary(t => t.key, t => t.Value);

                var model = new MyBusidex
                {
                    Busidex = sorted,
                    IsLoggedIn = User.Identity.IsAuthenticated,
                    TagCloud = tags,
                    CardCount = list.Count,
                    ImageCDNPath = ConfigurationManager.AppSettings["userCardPath"]
                };

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        MyBusidex = model
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, id);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        MyBusidex = new MyBusidex
                        {
                            Busidex = new List<UserCard>(),
                            IsLoggedIn = User.Identity.IsAuthenticated
                        }
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public void Put(long id, bool isMobileView)
        {

            _cardRepository.UpdateMobileView(id, isMobileView);
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateUserCardStatus(OrganizationGuest guest)
        {
            var userId = ValidateUser();
            if (userId == 0)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            _cardRepository.UpdateUserCardStatus(guest.UserCardId, guest.AddStatus);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };

        }

        public IHttpActionResult Post(long userId, long cId)
        {
            try
            {
                if (userId <= 0)
                {
                    userId = ValidateUser();
                }

                if (userId == 0)
                {
                    return BadRequest();;
                }

                var myBusidex = _cardRepository.GetMyBusidex(userId, false);
                if (myBusidex.Any(c => c.CardId == cId))
                {
                    return Conflict();
                }
                _cardRepository.AddToMyBusidex(cId, userId);

                //TODO: make this asynchronous
                var eventSources = _cardRepository.GetAllEventSources();
                _cardRepository.AddEventActivity(new EventActivity
                {
                    CardId = cId,
                    UserId = userId,
                    ActivityDate = DateTime.UtcNow,
                    EventSourceId = eventSources.Single(s => s.EventCode == EventSources.ADD.ToString()).EventSourceId
                });
                return Ok();
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return InternalServerError();
            }
        }

        public void Delete(long id, long userId)
        {
            if (userId <= 0)
            {
                userId = ValidateUser();
                if (userId <= 0)
                {
                    return;
                }
            }

            try
            {
                UserCard uc = _cardRepository.GetUserCard(id, userId);
                if (uc != null)
                {
                    _cardRepository.DeleteUserCard(uc, userId);
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, id);
            }
        }
    }
}