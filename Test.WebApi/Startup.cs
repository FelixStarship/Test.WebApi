using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Test.WebApi.Providers;

[assembly: OwinStartup(typeof(Test.WebApi.Startup))]

namespace Test.WebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            ConfigureOAuth(app);
            //这一行代码必须放在ConfiureOAuth(app)之后
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider=new CNBlogsAuthorizationServerProvider(),
                RefreshTokenProvider=new SimpleRefreshTokenProvider()
            };
            app.UseOAuthBearerTokens(OAuthServerOptions);
            // Token Generation
            //app.UseOAuthAuthorizationServer(OAuthServerOptions);
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
