using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    public class SearchController : HttpClientBase
    {
        public async Task<SearchModel> Search(string query)
        {
            return await GetAsync<SearchModel>($"/search/{query}");
        }
    }
}
