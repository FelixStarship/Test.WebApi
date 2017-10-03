using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Test.WebApi.Auth;

namespace Test.WebApi.Providers
{
    //http://www.cnblogs.com/Leo_/wl/p/4919783.html
    /*
     ValidateClientAuthentication方法用来对third party application 认证，
     具体的做法是为third party application颁发appKey和appSecrect，
     在本例中我们省略了颁发appKey和appSecrect的环节，我们认为所有的third party application都是合法的，
     context.Validated(); 表示所有允许此third party application请求。
     GrantResourceOwnerCredentials方法则是resource owner password credentials模式的重点，
     由于客户端发送了用户的用户名和密码，所以我们在这里验证用户名和密码是否正确，
     后面的代码采用了ClaimsIdentity认证方式，其实我们可以把他当作一个NameValueCollection看待。
     最后context.Validated(ticket); 表明认证通过。
     只有这两个方法同时认证通过才会颁发token。
     TokenEndpoint方法将会把Context中的属性加入到token中。
     */

    public class SimpleAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (AuthRepository _repo = new AuthRepository())
             {
                 IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                 if (user == null)
                 {
                     context.SetError("invalid_grant", "The user name or password is incorrect.");
                     return;
                 }
             }

             var identity = new ClaimsIdentity(context.Options.AuthenticationType);
             identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
             identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
             identity.AddClaim(new Claim("sub", context.UserName));

             var props = new AuthenticationProperties(new Dictionary<string, string>
             {
                 {
                     "as:client_id", context.ClientId ?? string.Empty
                 },
                 {
                     "userName", context.UserName
                 }
             });

             var ticket = new AuthenticationTicket(identity, props);
             context.Validated(ticket);
        }
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
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

    }
}