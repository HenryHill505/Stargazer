using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Stargazer.Startup))]
namespace Stargazer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            RequestManager.GetLightPollutionData(45, -88, 50000, 3);
        }
    }
}
