using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WindAuth.App_Start;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(Startup))]

namespace WindAuth.App_Start
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // token generation
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                // for demo purposes
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new AuthorizationServerProvider(),
                AuthenticationType = OAuthDefaults.AuthenticationType
            });


            // token consumption
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseWebApi(WebApiConfig.Register());
            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
        }

    }
}