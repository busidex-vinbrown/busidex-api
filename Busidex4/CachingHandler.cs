using System;
using System.Web;

namespace Busidex4
{
    public class CachingHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string file = context.Server.MapPath(context.Request.FilePath);
            string filename = file.Substring(file.LastIndexOf('\\') + 1);
            string extension = file.Substring(file.LastIndexOf('.') + 1);
            context.Response.Cache.SetExpires(DateTime.Now.Add(new TimeSpan(360, 0, 0, 0))); // cache for 10 days
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetValidUntilExpires(false);
            context.Response.AddHeader("Expires", DateTime.Now.AddDays(360).ToString());
            context.Response.AddHeader("content-disposition", "inline; filename=" + filename);
            context.Response.AddHeader("Cache-Control", "public, max-age=360000");
            context.Response.Filter = new System.IO.Compression.GZipStream(context.Response.Filter, System.IO.Compression.CompressionMode.Compress);
            context.Response.AppendHeader("Content-Encoding", "gzip");
            context.Response.WriteFile(file);
        }

        #endregion
    }
}