using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Test.WebApi.Providers
{
    //http://www.cnblogs.com/dudu/p/4569857.html  dudu博客
    public class CNBlogsAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {

        //http://www.cnblogs.com/dudu/p/oauth-refresh-token.html  持久化Token
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.Rejected();
                return;
            }

            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);

            await base.GrantRefreshToken(context);
        }
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            if (clientId == "1234" && clientSecret == "5678")
            {
                context.Validated(clientId);
            }

            return base.ValidateClientAuthentication(context);
        }
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            //var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "iOS App"));
            //var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            //context.Validated(ticket);

            //return base.GrantClientCredentials(context);
            var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "as:client_id", context.ClientId }
                });
            var ticket = new AuthenticationTicket(oAuthIdentity, props);

            context.Validated(ticket);

            return base.GrantClientCredentials(context);
        }
    }
}