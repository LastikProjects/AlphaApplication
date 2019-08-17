using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AlphaApplication.Startup))]
namespace AlphaApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
