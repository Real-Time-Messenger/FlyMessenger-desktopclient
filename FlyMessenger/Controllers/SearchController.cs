using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using RestSharp;

namespace FlyMessenger.Controllers
{
    public class SearchController : HttpClientBase
    {
        public async Task<SearchModel> Search(string query)
        {
            return (await GetAsync<SearchModel>($"/search?query={query}")).Data!;
        }
        
        public async Task<SearchModel> SearchInDialog(string query, string id)
        {
            return (await GetAsync<SearchModel>($"/search/{id}?query={query}")).Data!;
        }
    }
}
