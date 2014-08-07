using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WindAuth.App_Start
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // OAuth2 supports the notion of client authentication
            // this is not used here

            await TaskEx.Run(() => { context.Validated(); });
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await Task.Run(() =>
            {
                if (context.UserName != "vincenzo" || context.Password != "ceccio")
                {
                    context.Rejected();
                    return;
                }


                // create identity
                var id = new ClaimsIdentity(context.Options.AuthenticationType);
                id.AddClaim(new Claim("sub", context.UserName));
                id.AddClaim(new Claim("role", "user"));

                context.Validated(id);
                context.Request.Context.Authentication.SignIn(id);

            });
        }

    }
}
