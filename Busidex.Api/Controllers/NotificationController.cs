using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    public class NotificationController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public NotificationController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
        }

        public HttpResponseMessage Get(long? id)
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
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            try
            {
                var notifications = new List<Notification<SharedCard>>();
                
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Notifications = notifications
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
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
