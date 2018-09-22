using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class GroupsController : BaseApiController
    {
        
        public GroupsController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long id)
        {
            if (id <= 0)
            {
                id = ValidateUser();
            }
            try
            {
                var model = _cardRepository.GetMyBusiGroups(id);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = model

                    }),
                    StatusCode = HttpStatusCode.OK
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
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post(long userId, int groupTypeId, long? id, string cardIds, string description)
        {

            if (userId <= 0)
            {
                userId = ValidateUser();
            }

            
            if (string.IsNullOrEmpty(cardIds) || string.IsNullOrEmpty(description))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var group = new Group
                {
                    Description = description,
                    OwnerId = userId,
                    GroupId = id.GetValueOrDefault(),
                    GroupTypeId = groupTypeId
                };


                if (id.GetValueOrDefault() <= 0)
                {
                    _cardRepository.AddGroup(group, cardIds);

                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Model = string.Empty

                        }),
                        StatusCode = HttpStatusCode.OK
                    };

                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Model = string.Empty

                }),
                StatusCode = HttpStatusCode.NotModified
            };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put(long userId, int groupTypeId, long? id, string cardIds, string description)
        {
            if (userId <= 0)
            {
                userId = ValidateUser();
            }

            if (string.IsNullOrEmpty(cardIds) || string.IsNullOrEmpty(description))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var group = new Group
                {
                    Description = description,
                    OwnerId = userId,
                    GroupId = id.GetValueOrDefault(),
                    GroupTypeId = groupTypeId
                };

                // Get the card info for each card in the list of cardIds
                var cardIdList = new List<long>();
                cardIdList.AddRange(cardIds.Split(',')
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Select(cardId => Convert.ToInt64(cardId)));

                id = _cardRepository.UpdateGroup(group);
                if (id.GetValueOrDefault() > 0)
                {
                    var groupCards = _cardRepository.GetBusiGroupCards(id.Value);

                    #region Add new Cards to the group

                    var newCardIds = new List<long>();
                    newCardIds.AddRange((from gc in cardIdList
                        where !groupCards.Exists(cl => cl.CardId == gc)
                        select gc).ToList());
                    if (newCardIds.Count > 0)
                    {
                        _cardRepository.AddGroupCards(id.Value, string.Join(",", newCardIds));
                    }

                    #endregion

                    #region  Process any cards that have been removed from the group

                    var existingCardIds = new List<long?>();
                    existingCardIds.AddRange((from gc in groupCards
                        select gc.CardId).ToList());

                    var deletedCardIds = new List<long?>();
                    deletedCardIds.AddRange((from gc in existingCardIds
                        where !cardIdList.Exists(cl => cl == gc.GetValueOrDefault())
                        select gc).ToList());

                    if (deletedCardIds.Count > 0)
                    {
                        _cardRepository.RemoveGroupCards(id.Value, string.Join(",", deletedCardIds), userId);
                    }

                    #endregion

                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Model = id

                        }),
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Model = id

                }),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage Delete(long id)
        {
            var userId = ValidateUser();
            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            if (id <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            try
            {
                _cardRepository.DeleteGroup(id);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = string.Empty

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
                        Model = string.Empty

                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
