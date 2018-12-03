using System.Web.Http;
using System.Web.Http.Results;

namespace Busidex.Api.Controllers
{
    public class HomeController : ApiController
    {
        [Route("home")]
        public JsonResult<string> Get()
        {
            return Json("Busidex API");
        }
    }
}
