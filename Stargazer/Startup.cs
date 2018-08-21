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
            var x = RequestManager.PostImgur("C:/Users/Henry/Desktop/devCodeFiles/c#/Stargazer/Stargazer/App_Data/Test.PNG");
        }
    }
}
