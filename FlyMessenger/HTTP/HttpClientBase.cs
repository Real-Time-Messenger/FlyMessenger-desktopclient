using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
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
                {"Content-type", "application/json"},
                // TODO: Get authorization token from current session. 
                {"Authorization", GetToken()}
            });
        }

        protected T Get<T>(string url)
        {
            var request = new RestRequest(url);
            
            var response = _client.Execute<T>(request);
            return response.Data!;
        }
        
        public static string GetToken()
        {
            var fileStream = new FileStream("../../../Config/Token.txt", FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream);
            var token = "Bearer" + " " + streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();
            
            return token;
        }
        
        public static void SetToken(string token)
        {
            var fileStream = new FileStream("../../../Config/Token.txt", FileMode.Create, FileAccess.Write);
            var streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(token);
            streamWriter.Close();
            fileStream.Close();
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
        
        protected T Put<T>(string url, byte[] file) where T : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddHeader("Content-type", "multipart/form-data");
            request.AddFile("file", file, "image.jpg");

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
