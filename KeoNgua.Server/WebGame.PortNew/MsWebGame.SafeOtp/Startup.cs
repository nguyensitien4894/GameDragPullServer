using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using System.Configuration;
using System.Web.Cors;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(MsWebGame.SafeOtp.Startup))]

namespace MsWebGame.SafeOtp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.UseCors(SignalrCorsOptions.Value);
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
