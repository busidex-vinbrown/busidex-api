using System;
using System.Web;
using System.Web.Mvc;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    public class ImagePreviewResult : ActionResult
    {
        private readonly string _imageSrc;
        public ImagePreviewResult(string base64string)
        {
            _imageSrc = base64string;
        }


        public override void ExecuteResult( ControllerContext context )
        {
            var response = context.RequestContext.HttpContext.Response;

            response.Buffer = false;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.ContentEncoding = System.Text.Encoding.UTF8;
            response.ContentType = "text/plain";
            response.Write(_imageSrc);

            if ( response.IsClientConnected )
            {
                response.Flush();
                response.End();
            }
        }
    }
}