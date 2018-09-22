using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class GroupDetailController : BaseApiController
    {

        public GroupDetailController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long id)
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
            var cards = _cardRepository.GetBusiGroupCards(id, false);
            var group = _cardRepository.GetBusiGroupById(id);
            var model = new BusiGroupModel
            {
                Busigroup = group,
                BusigroupCards = cards
            };

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

        
    }
}
