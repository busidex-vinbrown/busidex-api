using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Busidex.Profile
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "SiteMapDefault",
               url: "",
               defaults: new { controller = "Home", action = "SiteMap", name = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "SiteMap",
               url: "SiteMap",
               defaults: new { controller = "Home", action = "SiteMap", name = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "details/{name}",
                defaults: new { controller = "Home", action = "Index", name = UrlParameter.Optional }
            );

           
        }
    }
}
