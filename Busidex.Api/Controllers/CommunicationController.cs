using System;
//using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
//using Google.GData.Contacts;
//using Google.GData.Client;
//using Google.GData.Extensions;
//using Google.Contacts;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class CommunicationsController : BaseApiController
    {

        //private readonly IAccountRepository _accountRepository;
        //private string _contactsEndpoint;

        public class CommunicationParams
        {
            public string[] EmailList { get; set; }
            public long UserId { get; set; }
        }

        public CommunicationsController(ICardRepository cardRepository)

        {
            //_accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public HttpResponseMessage Post([FromBody] CommunicationParams p)
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
            try
            {
                var communications = _cardRepository.GetCommunications(p.EmailList, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                        Communications = communications.ToDictionary(c=>c.Email, c=> c)
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
                        StatusCode = HttpStatusCode.InternalServerError
                    })
                };
            }
            

        }
    }
}
