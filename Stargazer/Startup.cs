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
            RequestManager.GetLightPollutionData(43, -87, 40000, 4);
        }
    }
}
