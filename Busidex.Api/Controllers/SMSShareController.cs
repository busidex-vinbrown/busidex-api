using System;
using System.Net;
using System.Net.Http;
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
    public class SmsShareController : BaseApiController
    {
        private readonly ISMSShareRepository _smsShareRepository;

        public SmsShareController(ICardRepository cardRepository, ISMSShareRepository smsShareRepository)
        {
            _cardRepository = cardRepository;
            _smsShareRepository = smsShareRepository;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody] SMSShare model)
        {
            var userId = ValidateUser();

            if (userId <= 0)
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

            if (model == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest
                    })
                };
            }

            try
            {
                _smsShareRepository.SaveSmsShare(model);

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
                _cardRepository.SaveApplicationError(ex, userId);
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
    }
}
