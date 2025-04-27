using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Cors;

[assembly: OwinStartup(typeof(HorseHunter.Server.App_Start.Startup))]
namespace HorseHunter.Server.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            // make the client reconnect.
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            // Wait a maximum of 30 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

            // For transports other than long polling, send a keepalive packet every
            // 10 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

            var hubConfiguration = new HubConfiguration()
            {
                EnableJSONP = true,
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true
            };

            app.UseCors(SignalrCorsOptions.Value);
            app.MapSignalR(hubConfiguration);
        }

        private static readonly Lazy<CorsOptions> SignalrCorsOptions = new Lazy<CorsOptions>(() =>
        {
            return new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context =>
                    {
                        var policy = new CorsPolicy
                        {
                            AllowAnyMethod = true,
                            AllowAnyHeader = true,
                            SupportsCredentials = true
                        };

                        string[] Domains = ConfigurationManager.AppSettings["AllowedDomains"].Split(',');
                        // Add allowed origins.
                        foreach (var item in Domains)
                        {
                            policy.Origins.Add(item);
                        }

                        return Task.FromResult(policy);
                    }
                }
            };
        });
    }
}