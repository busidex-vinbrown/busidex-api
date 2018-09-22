using System.Web.Http;
using AttributeRouting.Web.Http.WebHost;
using Busidex.Api.Controllers;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.HttpPipeline;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Busidex.Api.AttributeRoutingHttpConfig), "Start")]

namespace Busidex.Api 
{
    public static class AttributeRoutingHttpConfig
    {
		public static void RegisterRoutes(HttpRouteCollection routes) 
		{    
			// See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			// To debug routes locally using the built in ASP.NET development server, go to /routes.axd

            routes.MapHttpAttributeRoutes();
		}

        //public static void Start() 
        //{
        //    RegisterRoutes(GlobalConfiguration.Configuration.Routes);
        //}

        public static void Start()
        {
            Configure(GlobalConfiguration.Configuration, false);
        }

        public static void Configure(HttpConfiguration config, bool runInMemory)
        {
            // Register RESTful URLs
            config.Routes.MapHttpAttributeRoutes(cfg =>
            {
                cfg.InMemory = runInMemory;
                cfg.AddRoutesFromAssemblyOf<BaseApiController>();
            });

            // Handle all exceptions the same way by returning JSON data (serialized ErrorMessage object)
            config.Filters.Add(new ApiExceptionFilterAttribute());

            // Handle OAuth 2.0 authentication
            config.MessageHandlers.Add(new OAuth2AccessTokenHandler());

            // Permit text/plain and text/html content types in responses
            config.Formatters.Add(new TextFormatter());

            // Permit URL-encoded payload parameters to be bound to controller method args
            //config.ParameterBindingRules.Insert(0, MfcValueActionBinder.HookupParameterBinding);

            // Configure internal-to-public service model mappings
            //ModelMapperConfig.Intialize();
        }
    }
}
