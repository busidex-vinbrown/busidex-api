using System.Web.Http;
using Newtonsoft.Json.Converters;

namespace Busidex.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.MapHttpAttributeRoutes();
            config.Formatters.XmlFormatter.UseXmlSerializer = false;
            config.EnsureInitialized();

            //config.Routes.MapHttpRoute(
            //    name: "LoginRoute",
            //    routeTemplate: "api/{controller}/{action}/{name}/{pswd}",
            //    defaults: new { name = RouteParameter.Optional, pswd = RouteParameter.Optional }
            //    );
            config.Routes.MapHttpRoute(
                name: "LoginRoutePost",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            //config.Routes.MapHttpRoute(
            //    name: "RegistrationApi",
            //    routeTemplate: "api/{controller}/{token}",
            //    defaults: new { token = RouteParameter.Optional }
            //    );
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            GlobalConfiguration.Configuration.Formatters
                .JsonFormatter
                .SerializerSettings
                .Converters.Add(new StringEnumConverter());
            

           
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            
        }
    }
}
