using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Http.Cors;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class MyCardController : BaseApiController
    {

        public MyCardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Mine(long id)
        {
            if (id == 0)
            {
                id = ValidateUser();

                if (id <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            StatusCode = HttpStatusCode.NotFound
                        })
                    };
                }
            }

            var cards = _cardRepository.GetCardsByOwnerId(id);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    MyCards = cards.Where(c=> c.FrontFileId != null).ToList()
                })
            };
        }
    }
}
