using System;
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
    public class EmailTemplateController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public EmailTemplateController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
        }

       
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(EmailTemplateCode code)
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

                var template = _accountRepository.GetEmailTemplate(code);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Template = template
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
    }
}
