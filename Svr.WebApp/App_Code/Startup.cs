using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Svr.WebApp.Startup))]
namespace Svr.WebApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
