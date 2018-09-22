using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class ApplicationVersionController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var version = new AppVersion
            {
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };

            return new HttpResponseMessage
            {
                Content = new JsonContent(version),
                StatusCode = HttpStatusCode.OK
            };
        }
    }

    public class AppVersion
    {
        public string Version { get; set; }
    }
}
