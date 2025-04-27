using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MsWebGame.Facebook.Startup))]
namespace MsWebGame.Facebook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
