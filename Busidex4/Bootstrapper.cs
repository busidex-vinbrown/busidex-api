using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using Busidex.DAL;
using Busidex.BL;

namespace Busidex4
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // e.g. container.RegisterType<ITestService, TestService>();            

            //container.RegisterControllers();

            container.RegisterType<IBusidexDataContext, BusidexDataContext>(new InjectionConstructor());
            container.RegisterType<ICardRepository, CardRepository>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<ISettingsRepository, SettingsRepository>();
            container.RegisterType<IApplicationRepository, RepositoryBase>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            
            return container;
        }
    }
}