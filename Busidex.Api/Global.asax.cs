using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
//using Busidex.Api.App_Start;
using log4net.Core;
using Microsoft.WindowsAzure.Storage.Auth;
using Ms.Azure.Logging.Helpers;

namespace Busidex.Api
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            Bootstrapper.Initialise();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //var credentials = new StorageCredentials("busidexlog", "090mM+/eHvDYIup3lkVyGHIXJF8oX3/v4g3n/zhxPIYPrhtSMjAEqSfDIt/RQaBEUpCAscFJMetrliSULbIgXg==");
            //var account = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(credentials, true);

            //LoggingHelper.InitializeAzureTableLogging(account, "busidexlogging", Level.All);
            //LoggingHelper.InitializeFromConfiguration();
            log4net.Config.XmlConfigurator.Configure(); 
            
        }

        protected void Application_End()
        {
            LoggingHelper.FlushAppenders();
        }
    }
}