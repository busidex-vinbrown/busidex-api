using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Http.Cors;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;
using System.Threading.Tasks;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class SearchController : BaseApiController
    {

        private readonly IOrganizationRepository _organizationRepository;

        public SearchController(ICardRepository cardRepository, IOrganizationRepository organizationRepository)
        {
            _cardRepository = cardRepository;
            _organizationRepository = organizationRepository;
        }
        
        [System.Web.Http.HttpPost]
        public HttpResponseMessage DoSearch(long? userId, [FromBody] MobileSearchResultModel model)
        {
            try
            {
                var newModel = new SearchResultModel
                {
                    CardType = CardType.Professional,
                    Criteria = model.Criteria,
                    Display = model.Display,
                    Distance = model.Distance,
                    HasResults = model.HasResults,
                    IsLoggedIn = model.IsLoggedIn,
                    Results = model.Results,
                    SearchAddress = model.SearchAddress,
                    SearchLocation = model.SearchLocation,
                    SearchText = model.SearchText,
                    TagCloud = model.TagCloud,
                    UserId = userId
                };
                return Search(newModel);
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        SearchModel = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Search([FromBody] SearchResultModel model)
        {
            long userId = 0;
            try
            {
                if (model == null)
                {
                  //  _cardRepository.SaveApplicationError(new Exception("Search Model is Null"), userId);
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false
                        }),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                userId = model.UserId.GetValueOrDefault() == 0 ? ValidateUser() : model.UserId.GetValueOrDefault();
                if (model.UserId.GetValueOrDefault() == 0)
                {
                    model.UserId = userId;
                }
                SearchResultModel searchModel = _cardRepository.Search(model, userId, null);

               // _cardRepository.SaveApplicationError(new Exception("TEST: " + model.CardType.ToString() + ", " + model.UserId.GetValueOrDefault().ToString() + ", " + searchModel.Results.Count.ToString()), userId);

                var sorted = new List<CardDetailModel>();
                sorted.AddRange(searchModel.Results.Where(c => c.OwnerId.GetValueOrDefault() == userId));
                sorted.AddRange(
                    searchModel.Results
                    .Where(c => c.OwnerId != userId)
                        .OrderByDescending(c => c.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Name)
                        .ThenBy(c => c.CompanyName)
                        .ToList());

                searchModel.Results = sorted;

                searchModel.IsLoggedIn = userId > 0;
                

                List<string> allTags = (from cards in searchModel.Results
                    from tag in cards.Tags
                    select tag.Text).ToList();

                var tags = (from tag in allTags
                    group tag by tag
                    into t
                    select new {key = t.First(), Value = t.Count()})
                    .ToDictionary(t => t.key, t => t.Value);

                searchModel.TagCloud = tags;
                searchModel.SearchText = model.SearchText;

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        SearchModel = searchModel
                    })
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
                        SearchModel = string.Empty
                    })
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage SystemTagSearch(string systag, bool ownedOnly = false)
        {
            long userId = 0;
            try
            {
                userId = ValidateUser();

                SearchResultModel searchModel = _cardRepository.SearchBySystemTag(systag, userId);

                if (ownedOnly)
                {
                    searchModel.Results.RemoveAll(c => !c.OwnerId.HasValue);
                }
                searchModel.IsLoggedIn = userId > 0;

                List<string> allTags = (from cards in searchModel.Results
                                        from tag in cards.Tags
                                        select tag.Text).ToList();

                var tags = (from tag in allTags
                            group tag by tag
                                into t
                                select new { key = t.First(), Value = t.Count() })
                    .ToDictionary(t => t.key, t => t.Value);

                searchModel.TagCloud = tags;
                searchModel.SearchText = systag;

                var sorted = new List<CardDetailModel>();
                sorted.AddRange(searchModel.Results.Where(c => c.OwnerId.GetValueOrDefault() == userId));
                sorted.AddRange(
                    searchModel.Results
                    .Where(c => c.OwnerId != userId)
                        .OrderByDescending(c => c.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Name)
                        .ThenBy(c => c.CompanyName)
                        .ToList());

                searchModel.Results = sorted;

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        SearchModel = searchModel
                    })
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
                        SearchModel = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage OrganizationMemberSearch(long orgId)
        {
            long userId = 0;
            try
            {
                userId = ValidateUser();

                var members = _organizationRepository.GetOrganizationCards(orgId, userId, false);

                var searchModel = new SearchResultModel
                {
                    Results = members,
                    IsLoggedIn = userId > 0
                };

                List<string> allTags = (from cards in searchModel.Results
                                        from tag in cards.Tags
                                        select tag.Text).ToList();

                var tags = (from tag in allTags
                            group tag by tag
                                into t
                                select new { key = t.First(), Value = t.Count() })
                    .ToDictionary(t => t.key, t => t.Value);

                searchModel.TagCloud = tags;
                searchModel.SearchText = string.Empty;

                var sorted = new List<CardDetailModel>();
                sorted.AddRange(searchModel.Results.Where(c => c.OwnerId.GetValueOrDefault() == userId));
                sorted.AddRange(
                    searchModel.Results
                    .Where(c => c.OwnerId != userId)
                        .OrderByDescending(c => c.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Name)
                        .ThenBy(c => c.CompanyName)
                        .ToList());

                searchModel.Results = sorted;

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        SearchModel = searchModel
                    })
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
                        SearchModel = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage GroupCardSearch(string groupName)
        {
            long userId = 0;
            try
            {
                userId = ValidateUser();
                Group group = _cardRepository.GetGroupByName(groupName);

                SearchResultModel searchModel = _cardRepository.SearchByGroupName(groupName, userId);

                searchModel.IsLoggedIn = userId > 0;

                List<string> allTags = (from cards in searchModel.Results
                                        from tag in cards.Tags
                                        select tag.Text).ToList();

                var tags = (from tag in allTags
                            group tag by tag
                                into t
                                select new { key = t.First(), Value = t.Count() })
                    .ToDictionary(t => t.key, t => t.Value);

                searchModel.TagCloud = tags;
                searchModel.SearchText = groupName;

                var sorted = new List<CardDetailModel>();
                sorted.AddRange(searchModel.Results.Where(c => c.OwnerId.GetValueOrDefault() == group.OwnerId));
                sorted.AddRange(
                    searchModel.Results
                    .Where(c => c.OwnerId != group.OwnerId)
                        .OrderByDescending(c => c.OwnerId.GetValueOrDefault() > 0 ? 1 : 0)
                        .ThenBy(c => c.Name)
                        .ThenBy(c => c.CompanyName)
                        .ToList());

                searchModel.Results = sorted;

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        SearchModel = searchModel
                    })
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
                        SearchModel = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<HttpResponseMessage> GetUserEventTags()
        {
            try
            {
                var userId = ValidateUser();

                var eventTags = _cardRepository.GetEventTags().OrderByDescending(e => e.EventTagId);

                var results = new List<EventTag>();
                foreach(var eventTag in eventTags)
                {
                    var searchModel = _cardRepository.SearchBySystemTag(eventTag.Text, userId);
                    if (searchModel.Results.Any())
                    {
                        results.Add(eventTag);
                    }
                }

                var response = new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = results
                    })
                };
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                await _cardRepository.SaveApplicationError(ex, 0);

                var response = new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Model = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
                return await Task.FromResult(response);
            }
        }

        public HttpResponseMessage GetEventTags()
        {
            try
            {

                var eventTags = _cardRepository.GetEventTags().OrderByDescending(e => e.EventTagId);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = eventTags
                    })
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
                        Model = string.Empty
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
