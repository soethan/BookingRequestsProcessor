using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BackOfficeWeb.Startup))]
namespace BackOfficeWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
