using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MsWebGame.CSKH.Startup))]
namespace MsWebGame.CSKH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // ConfigureAuth(app);
        }
    }
}
