using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Controllers
{
    [RequireHttps]   
    [EnableCors("*", "*", "*")]
    public class SuggestionsController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public SuggestionsController(IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get()
        {
            try
            {
                var userId = ValidateUser();
                var bu = _accountRepository.GetBusidexUserById(userId);
                var isAdmin = userId > 0 && Roles.IsUserInRole(bu.UserName, "Administrator");

                List<Suggestion> suggestionList = userId > 0
                    ? _accountRepository.GetAllSuggestions().Where(s => s.CreatedBy == userId || isAdmin).ToList()
                    : new List<Suggestion>();

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                        Suggestions = suggestionList,
                        Model = new Suggestion()
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "There was a problem getting suggestions." }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post(Suggestion suggestion)
        {
            try
            {
                suggestion.Votes = 1;
                List<Suggestion> suggestionList = _accountRepository.GetAllSuggestions();
                if (!string.IsNullOrEmpty(suggestion.Summary))
                {
                    _accountRepository.AddNewSuggestion(suggestion);

                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            StatusCode = HttpStatusCode.OK,
                            Suggestions = suggestionList,
                            Model = new Suggestion()
                        })
                    };
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Suggestions = suggestionList,
                        Model = new Suggestion()
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "There was a problem saving your suggestion." }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage Put(int? id = 0)
        {
            try
            {
                List<Suggestion> suggestionList = _accountRepository.GetAllSuggestions();
                if (id.GetValueOrDefault() > 0)
                {
                    _accountRepository.UpdateSuggestionVoteCount(id.GetValueOrDefault());
                    suggestionList = _accountRepository.GetAllSuggestions();
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            StatusCode = HttpStatusCode.OK,
                            Suggestions = suggestionList,
                            Model = new Suggestion()
                        })
                    };
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Suggestions = suggestionList,
                        Model = new Suggestion()
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "There was a problem saving your suggestion." }),
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
            }
        }
    }
}
