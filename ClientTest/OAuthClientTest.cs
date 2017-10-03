using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClientTest
{
    public class OAuthClientTest
    {
        private HttpClient _httpClient;

        public OAuthClientTest()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5706/");
        }

        public string Get_Accesss_Token_By_Client_Credentials_Grant()
        {  
            var clientId = "1234";
            var clientSecret = "5678";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");


            return _httpClient.PostAsync("/token", new FormUrlEncodedContent(parameters)).Result
                .Content.ReadAsStringAsync().Result.ToString();
        }

        public async Task<string> GetAccessTokenTest()
        {
            var clientId = "1234";
            var clientSecret = "5678";

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "client_credentials");
            parameters.Add("refresh_token", "47c854cf49c64537af838189fdf38f44");
            //parameters.Add("username", "[username]");
            //parameters.Add("password", "[password]");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));

            var response = await _httpClient.PostAsync("/token", new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            return responseValue;
        }
    }
}