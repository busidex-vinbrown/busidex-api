using System.Web.Http;
using Busidex.Api.DataAccess;
using Busidex.Api.DataServices;
using Busidex.Api.DataServices.Interfaces;
using Microsoft.Practices.Unity;
using AccountRepository = Busidex.Api.DataServices.AccountRepository;
using BusidexDataContext = Busidex.Api.DataAccess.BusidexDataContext;

namespace Busidex.Api
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // e.g. container.RegisterType<ITestService, TestService>();            
            container.RegisterType<IBusidexDataContext, BusidexDataContext>(new InjectionConstructor());
            container.RegisterType<ICardRepository, CardRepository>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<ISettingsRepository, SettingsRepository>();
            container.RegisterType<IApplicationRepository, RepositoryBase>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<IOrganizationRepository, OrganizationRepository>();
            container.RegisterType<IContactsRepository, ContactsRepository>();
            container.RegisterType<ISMSShareRepository, SmsShareRepository>();
            container.RegisterType<IUserDeviceRepository, UserDeviceRepository>();
            
            return container;
        }
    }
}