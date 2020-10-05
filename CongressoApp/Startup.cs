using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CongressoApp.Startup))]
namespace CongressoApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
