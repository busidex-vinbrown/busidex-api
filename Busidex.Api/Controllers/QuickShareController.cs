using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;
using System.Configuration;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class QuickShareController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly string _sharedCardStorageConnectionString;
        public QuickShareController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            _sharedCardStorageConnectionString = ConfigurationManager.AppSettings["BusidexQueuesConnectionString"];
        }

       
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody] SharedCard sharedCard)
        {
            try
            {
                long userId = ValidateUser();

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

                ProcessSharedCard(sharedCard, userId);

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

        void ProcessSharedCard(SharedCard sharedCard, long userId)
        {
            
            if (sharedCard.ShareWith.GetValueOrDefault() == 0)
            {
                var userAccount = _accountRepository.GetUserAccountByEmail(sharedCard.Email);
                if (userAccount != null)
                {
                    sharedCard.ShareWith = userAccount.UserId;
                } else
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
                _cardRepository.AddSharedCardToQueue(_sharedCardStorageConnectionString, sharedCard);
            } else
            {
                _cardRepository.SaveSharedCard(sharedCard);
            }
        }
    }
}
