using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace Busidex4.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class ReadOnlySessionController : Controller
    {

        //[OutputCache(VaryByParam = "*", Duration = 3600, Location = OutputCacheLocation.Downstream)]
        //public ActionResult Image(string fileId)
        //{
        //    Response.AddHeader("Expires", DateTime.Now.AddDays(360).ToString());
        //    Response.AddHeader("Cache-Control","max-age=360000");
        //    string cdnPath = ConfigurationManager.AppSettings["userCardPath"];

        //    var response = GetWebResponse(cdnPath + fileId);

        //    if (response != null)
        //    {
        //        using (var sr = new StreamReader(response.GetResponseStream()))
        //        {
                    
        //            var result = sr.ReadToEnd();
        //            byte[] bites = Convert.FromBase64String(result);
        //            return response;
        //        }
        //    }
        //    return File("", "image/jpeg");
        //}

        private HttpWebResponse GetWebResponse(string url)
        {
            //string authInfo = username + ":" + password;
            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            //request.Headers["Authorization"] = "Basic " + authInfo;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException webException)
            {
                string exception = webException.Message;
            }
            return response;
        }
    }

        
}
