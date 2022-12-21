using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;

namespace FlyMessenger.HTTP
{
    public class HttpClientBase
    {
        private readonly RestClient _client;

        protected HttpClientBase()
        {

            _client = new RestClient(Constants.ApiUrl).AddDefaultHeaders( new Dictionary<string, string>
            {
                {"Accept", "application/json"},
                {"Content-Type", "application/json"},
                // TODO: Get authorization token from current session. 
                {"Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NzQxMjgxNzUuMzMyMDMsImlhdCI6MTY3MTUzNjE3NS4zMzIwMywicGF5bG9hZCI6eyJpZCI6IjYzYTE5ZTJmMjQzNDkxMTdjNjI0NWZlNCJ9fQ.w3WxanKvZ-JAfn0C1sYOsulVW2xkbll_FOFAm1iNOkQ"}
            });
        }

        protected T Get<T>(string url)
        {
            var request = new RestRequest(url, Method.Get);

            var response = _client.Execute<T>(request);
            return response.Data!;
        }

        protected T Post<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Post);

            request.AddJsonBody(data);

            var response = _client.Execute<T>(request);
            return response.Data!;
        }

        protected T Put<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddJsonBody(data);

            var response = _client.Execute<T>(request);
            return response.Data!;
        }

        protected void Delete(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            _client.Execute(request);
        }
        
        protected async Task<T> GetAsync<T>(string url)
        {
            var request = new RestRequest(url);

            var response = await _client.ExecuteAsync<T>(request);
            return response.Data!;
        }
    }
}
