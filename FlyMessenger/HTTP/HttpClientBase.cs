using System.Collections.Generic;
using System.Threading.Tasks;
using FlyMessenger.Core.Utils;
using RestSharp;

namespace FlyMessenger.HTTP
{
    /// <summary>
    /// Http client base.
    /// </summary>
    public class HttpClientBase
    {
        private static RestClient? _client;

        /// <summary>
        /// Constructor.
        /// </summary>
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

        /// <summary>
        /// Sends a HTTP GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <returns>The server response as a <see cref="RestResponse{T}"/> object.</returns>
        protected RestResponse<T> Get<T>(string url)
        {
            var request = new RestRequest(url);

            var response = _client.Execute<T>(request);
            return response;
        }

        /// <summary>
        /// Gets the token from the token file.
        /// </summary>
        /// <returns>The token.</returns>
        public static string GetToken()
        {
            var tokenSettings = new TokenSettings();
            var token = tokenSettings.Load();

            return "Bearer " + token;
        }

        /// <summary>
        /// Updates the authorization header with the provided token.
        /// </summary>
        /// <param name="token">The token to set.</param>
        private static void UpdateAuthorizationHeader(string token)
        {
            _client?.DefaultParameters.RemoveParameter("Authorization");
            _client?.AddDefaultHeader("Authorization", "Bearer " + token);
        }

        /// <summary>
        /// Sets the token into the token file and updates the authorization header.
        /// </summary>
        /// <param name="token">The token to set.</param>
        public static void SetToken(string token)
        {
            var tokenSettings = new TokenSettings();
            tokenSettings.Save(token);

            UpdateAuthorizationHeader(token);
        }

        /// <summary>
        /// Sends a HTTP POST request to the specified URL with the provided data as JSON body.
        /// </summary>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <typeparam name="TK">The type of the data to send in the request body.</typeparam>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <returns>The server's response as a deserialized object of type RestResponse{T}.</returns>
        protected RestResponse<T> Post<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Post);

            request.AddJsonBody(data);

            var response = _client.Execute<T>(request);
            return response;
        }

        /// <summary>
        /// Sends a HTTP POST request to the specified URL.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <returns>The server's response as a deserialized object of type RestResponse.</returns>
        protected RestResponse Post(string url)
        {
            var request = new RestRequest(url, Method.Post);

            var response = _client.Execute(request);
            return response;
        }

        /// <summary>
        /// Sends a HTTP PUT async request to the specified URL with the provided data as JSON body.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <typeparam name="TK">The type of the data to send in the request body.</typeparam>
        /// <returns>The server's response as a deserialized object of type RestResponse with the specified type parameter.</returns>
        protected async Task<RestResponse<T>> PutAsync<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync<T>(request);
            return response;
        }

        /// <summary>
        /// Sends a HTTP PUT async request to the specified URL with the provided file as multipart/form-data.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <param name="file">The file to send in the request body.</param>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <returns>The server's response as a deserialized object of type RestResponse with the specified type parameter.</returns>
        protected async Task<T> PutAsync<T>(string url, byte[] file) where T : class
        {
            var request = new RestRequest(url, Method.Put);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", file, "image.jpg");

            var response = await _client.ExecuteAsync<T>(request);
            return response.Data!;
        }

        /// <summary>
        /// Sends a HTTP DELETE request to the specified URL.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        protected void Delete(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            _client.Execute(request);
        }

        /// <summary>
        /// Sends a HTTP DELETE async request to the specified URL.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        protected async Task DeleteAsync(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            await _client.ExecuteAsync(request);
        }

        /// <summary>
        /// Sends a HTTP GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <returns>The server's response as a deserialized object of type RestResponse with the specified type parameter.</returns>
        protected async Task<RestResponse<T>> GetAsync<T>(string url)
        {
            var request = new RestRequest(url);

            var response = await _client.ExecuteAsync<T>(request);
            return response;
        }

        /// <summary>
        /// Sends a HTTP POST request to the specified URL with the provided data as JSON body.
        /// </summary>
        /// <param name="url">The URL of the server to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <typeparam name="T">The type of the expected server response.</typeparam>
        /// <typeparam name="TK">The type of the data to send in the request body.</typeparam>
        /// <returns>The server's response as a deserialized object of type RestResponse with the specified type parameter.</returns>
        protected async Task<RestResponse<T>> PostAsync<T, TK>(string url, TK data) where T : class where TK : class
        {
            var request = new RestRequest(url, Method.Post);

            request.AddJsonBody(data);

            var response = await _client?.ExecuteAsync<T>(request)!;
            return response;
        }
    }
}
