using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Test.WebApi.Auth;

namespace Test.WebApi.Providers
{

    /*
         我们实现了其中两个异步方法，对两个同步方法不做实现。
         其中CreateAsync用来生成RefreshToken值，生成后需要持久化在数据库中，
         客户端需要拿RefreshToken来请求刷新token，
         此时ReceiveAsync方法将拿客户的RefreshToken和数据库中RefreshToken做对比，
         验证成功后删除此refreshToken。
    */
    public class SimpleRefreshTokenProvider: IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (AuthRepository _repo = new AuthRepository())
            {

                /*  var token = new RefreshToken()
                  {
                      Id = refreshTokenId.GetHash(),
                      Subject = context.Ticket.Identity.Name,
                      IssuedUtc = DateTime.UtcNow,
                      ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                  };

                  context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                  context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                  token.ProtectedTicket = context.SerializeTicket();

                  var result = await _repo.AddRefreshToken(token);

                  if (result)
                  {
                      context.SetToken(refreshTokenId);
                  }
                  */
                string tokenValue = Guid.NewGuid().ToString("n");

                context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
                context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

                _refreshTokens[tokenValue] = context.SerializeTicket();

                context.SetToken(tokenValue);

            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            /*
            string hashedTokenId = context.Token.GetHash();

            using (AuthRepository _repo = new AuthRepository())
            {
                var refreshToken = await _repo.FindRefreshToken(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await _repo.RemoveRefreshToken(hashedTokenId);
                }
            }*/

            string value;
            if (_refreshTokens.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}