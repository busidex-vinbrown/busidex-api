using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

//using Busidex4.App_Start;

namespace Busidex4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional});

            routes.MapHttpRoute(
                name: "ApiRoute",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });

        }

        protected void Application_BeginRequest()
        {
            //Response.AppendHeader("Refresh", ((FormsAuthentication.Timeout.Minutes + 2) * 60).ToString());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // MvcHandler.DisableMvcResponseHeader = true;


            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.EnableOptimizations = true;
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //BundleTable.Bundles.RegisterTemplateBundles();
            //BundleTable.Bundles.Add(new Bundle("~/Scripts/Release"));
            //BundleTable.Bundles.Add(new Bundle("~/Scripts/Release/ThirdParty"));
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            Bootstrapper.Initialise();

            //  AuthConfig.RegisterAuth();

            RegisterApis(GlobalConfiguration.Configuration);
            BootstrapSupport.BootstrapBundleConfig.RegisterBundles(BundleTable.Bundles);
            //BootstrapMvcSample.ExampleLayoutsRouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterApis(HttpConfiguration config)
        {
            // Add JavaScriptSerializer  formatter instead - add at top to make default
            //config.Formatters.Insert(0, new JavaScriptSerializerFormatter());

            // Add Json.net formatter - add at the top so it fires first!
            // This leaves the old one in place so JsonValue/JsonObject/JsonArray still are handled
            //config.Formatters.Insert(0, new JsonNetFormatter());
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
        }

        protected void Session_Start()
        {

            Session["MyBusidex"] = null;

            Session["Categories"] = null;

        }


    }
}