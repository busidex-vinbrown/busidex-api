using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Controllers
{
    [System.Web.Mvc.RequireHttps]
    [EnableCors("*", "*", "*")]
    public class SharedCardController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public SharedCardController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
        }

		[HttpGet]
        public HttpResponseMessage Get()
        {
            var id = ValidateUser();

            if (id <= 0)
            {
                
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.Unauthorized
                    })
                };
            }
            try
            {
                var sharedCards = _cardRepository.GetSharedCards(id);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                        SharedCards = sharedCards
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
                        StatusCode = HttpStatusCode.InternalServerError
                    })
                };
            }
            
        }

		[HttpGet]
		[Route("sharedcards/history")]
	    public HttpResponseMessage GetHistory()
		{

			var history = new List<SharedCard>();
			var sharedCards = new List<SharedCard>();
			return new HttpResponseMessage
			{
				Content = new JsonContent(new
				{
					Success = true,
					StatusCode = HttpStatusCode.OK,
					SharedCards = sharedCards
				})
			};
		}

		[HttpPut]
        public HttpResponseMessage Put(SharedCardModel model)
        {
            if (model == null)
            {
                _cardRepository.SaveApplicationError(new Exception("Model is empty"), 0);
                return Request.CreateResponse(HttpStatusCode.BadRequest, false);
            }

            long sharedWithId = 0;
            try
            {
                var acceptedCardIds = model.AcceptedCardIdList.ToList();
                var declinedCardIds = model.DeclinedCardIdList.ToList();

                var id = ValidateUser();

                if (id > 0)
                {
                    var eventSources = _cardRepository.GetAllEventSources();
                    var eventSourceId =
                        eventSources.Single(s => s.EventCode == EventSources.SHARE.ToString())
                            .EventSourceId;

                    sharedWithId = model.UserId == 0 ? id : model.UserId;

                    foreach (var cardId in acceptedCardIds)
                    {
                        _cardRepository.AcceptSharedCard(cardId, sharedWithId);
                        
                        _cardRepository.AddEventActivity(new EventActivity
                        {
                            CardId = cardId,
                            UserId = sharedWithId,
                            ActivityDate = DateTime.UtcNow,
                            EventSourceId = eventSourceId
                        });
                       
                    }

                    foreach (var cardId in declinedCardIds)
                    {
                        _cardRepository.DeclineSharedCard(cardId, sharedWithId);
                    }
                    _cardRepository.InvalidateBusidexCache();
                }

                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, sharedWithId);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, false);
            }
        }

        [HttpPost]
        public HttpResponseMessage Preview(SharedCard model)
        {
            if (model == null)
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

            var template = _cardRepository.GetSharedCardEmailPreview(model);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Template = template
                   
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [HttpPost]
        public HttpResponseMessage SendTestEmail([FromBody] SharedCard sharedCard)
        {
            try
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
                
                if (sharedCard == null)
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

                var user = _accountRepository.GetBusidexUserById(userId);
                sharedCard.ShareWith = userId;
                sharedCard.Email = user.Email;
                
                
                if (!System.Diagnostics.Debugger.IsAttached && !Request.RequestUri.Host.Contains("local"))
                {
                    _cardRepository.SendSharedCardInvitation(sharedCard);
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
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] List<SharedCard> sharedCards)
        {
            try
            {
                long userId = ValidateUser();

                if (sharedCards == null || sharedCards.Count == 0)
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

                ProcessSharedCards(sharedCards, userId);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
                
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        void ProcessSharedCards(List<SharedCard>  sharedCards, long userId)
        {
            var sharedCardList = new List<SharedCard>();
            
            // build a list of shared cards. cards shared by phone number could match multiple recipients, so send to all of them.
            foreach (var sc in sharedCards)
            {
                if (!string.IsNullOrEmpty(sc.Email))
                {
                    sharedCardList.Add(sc);
                }               
            }

            if (sharedCardList.Count > 0)
            {
                // Need to handle multiple recipients
                var recipients = sharedCardList.First().Email.Split(',');

                foreach (var recipient in recipients)
                {                    

                    foreach (var sharedCard in sharedCardList)
                    {
                        sharedCard.Email = recipient.Trim();

                        if (sharedCard.ShareWith.GetValueOrDefault() == 0)
                        {
                            var userAccount = _accountRepository.GetUserAccountByEmail(sharedCard.Email);
                            if (userAccount != null)
                            {
                                sharedCard.ShareWith = userAccount.UserId;
                            }
                            else
                            {
                                var card = _cardRepository.GetCardByEmail(sharedCard.Email);
                                if (card != null && card.OwnerId.GetValueOrDefault() > 0)
                                {
                                    sharedCard.ShareWith = card.OwnerId;
                                }
                            }
                        }
                        if (userId > 0 && sharedCard.SendFrom == 0)
                        {
                            sharedCard.SendFrom = userId;
                        }
                        if (!System.Diagnostics.Debugger.IsAttached && !Request.RequestUri.Host.Contains("local"))
                        {
                            _cardRepository.AddSharedCardToQueue(sharedCard);
                        }
                        else
                        {
                            _cardRepository.SaveSharedCard(sharedCard);
                        }
                    }
                }
            }
        }

    //    [System.Web.Http.HttpGet]
    //    public HttpResponseMessage Get30DaySharedCardReminder()
    //    {

    //        var sharedCards = _cardRepository.Get30DaySharedCards();

    //        return new HttpResponseMessage
    //        {
    //            Content = new JsonContent(new
    //            {
    //                Success = true,
    //                SharedCards = sharedCards
    //            }),
    //            StatusCode = HttpStatusCode.OK
    //        };

    //    }

    //    [System.Web.Http.HttpGet]
    //    public HttpResponseMessage Send30DaySharedCardReminder()
    //    {

    //        long userId = ValidateUser();

    //        if (userId <= 0)
    //        {
    //            return new HttpResponseMessage
    //            {
    //                Content = new JsonContent(new
    //                {
    //                    Success = false,
                        
    //                }),
    //                StatusCode = HttpStatusCode.Unauthorized
    //            };
    //        }

    //        var sharedCards = _cardRepository.Get30DaySharedCards();

    //        ProcessSharedCards(sharedCards, userId);

    //        return new HttpResponseMessage
    //        {
    //            Content = new JsonContent(new
    //            {
    //                Success = true
                    
    //            }),
    //            StatusCode = HttpStatusCode.OK
    //        };
    //    }
    }
}
