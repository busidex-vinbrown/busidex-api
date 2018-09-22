using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Busidex.Profile.Startup))]
namespace Busidex.Profile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
