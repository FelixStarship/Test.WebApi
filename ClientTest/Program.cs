using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {   
            //测试客户端验证
            OAuthClientTest oAuthClientTest = new OAuthClientTest();
            //var access_Token=oAuthClientTest.Get_Accesss_Token_By_Client_Credentials_Grant();
           var refresh_token= oAuthClientTest.GetAccessTokenTest().Result;
        }
    }
}
