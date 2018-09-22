using System;
using System.Web.Mvc;

namespace Busidex4.Controllers
{
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
            //response.Write("/*");
            //response.Write("Content-Type: multipart/related; boundary=\"_\"");
            //response.Write(Environment.NewLine);
            //response.Write("--");
            //response.Write("_");
            //response.Write("Content-Location:1");
            //response.Write("Content-Type: image/png");
            //response.Write("Content-Transfer-Encoding:base64");
            //response.Write(Environment.NewLine);
            //response.Write(_imageSrc);
            //response.Write(Environment.NewLine);
            //response.Write("--_--");
            //response.Write("*/");
            //response.Write("body {");
            //response.Write("background: transparent url(data:image/png;base64," + _imageSrc + ") repeat-x top left;");
            //response.Write("*background: transparent url('mhtml:http://absolute_url.com/css_file.css!1') repeat-x top left;");
            //response.Write("}");

            if ( response.IsClientConnected )
            {
                response.Flush();
                response.End();
            }
        }
    }
}