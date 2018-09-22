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
    public class GroupNotesController : BaseApiController
    {

        public GroupNotesController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public HttpResponseMessage Put(long id, string notes)
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

            _cardRepository.SaveGroupCardNotes(id, notes);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Model = string.Empty
                })
            };
        }

    }
}
