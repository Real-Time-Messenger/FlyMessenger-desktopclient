using System.Collections.Generic;
using System.Threading.Tasks;
using FlyMessenger.Core.Utils;
using RestSharp;

namespace FlyMessenger.HTTP
{
    public class HttpClientBase
    {
        private static RestClient? _client;

        protected HttpClientBase()
        {
            _client = new RestClient(Constants.ApiUrl).AddDefaultHeaders(
                new Dictionary<string, string>
                {
                    { "Accept", "application/json" },
                    { "Content-type", "application/json" },
                    { "X-Client-Name", "FlyMessenger" },
                    { "X-Client-Version", "0.9.0" },
                    { "X-Client-Type", "Desktop" },
                    { "Authorization", GetToken() }
                }
            );
        }

        protected RestResponse<T> Get<T>(string url) {
            var request = new RestRequest(url);

            var response = _client.Execute<T>(request);
            return response;
        }

        public static string GetToken()
        {
            var tokenSettings = new TokenSettings();
            var token = tokenSettings.Load();

            return "Bearer " + token;
        }

        private static void UpdateAuthorizationHeader(string token)
        {
            _client?.DefaultParameters.RemoveParameter("Authorization");
            _client?.AddDefaultHeader("Authorization", "Bearer " + token);
        }

        public static void SetToken(string token)
        {
            var tokenSettings = new TokenSettings();
            tokenSettings.Save(token);
            
            UpdateAuthorizationHeader(token);
        }

        protected RestResponse<T> Post<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Post);

            request.AddJsonBody(data);

            var response = _client.Execute<T>(request);
            return response;
        }
        
        protected RestResponse Post(string url)
        {
            var request = new RestRequest(url, Method.Post);

            var response = _client.Execute(request);
            return response;
        }

        protected RestResponse<T> Put<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddJsonBody(data);

            var response = _client.Execute<T>(request);
            return response;
        }

        protected T Put<T>(string url, byte[] file) where T : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", file, "image.jpg");

            var response = _client.Execute<T>(request);
            return response.Data!;
        }

        protected void Delete(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            _client.Execute(request);
        }

        protected async Task<RestResponse<T>> GetAsync<T>(string url)
        {
            var request = new RestRequest(url);

            var response = await _client.ExecuteAsync<T>(request);
            return response;
        }
        
        protected async Task<RestResponse<T>> PostAsync<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Post);

            request.AddJsonBody(data);

            var response = await _client?.ExecuteAsync<T>(request)!;
            return response;
        }
    }
}
