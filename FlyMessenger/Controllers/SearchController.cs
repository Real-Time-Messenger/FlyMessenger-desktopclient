using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    /// <summary>
    /// Search controller.
    /// </summary>
    public class SearchController : HttpClientBase
    {
        // Search method for searching all.
        public async Task<SearchModel> Search(string query)
        {
            return (await GetAsync<SearchModel>($"/search?query={query}")).Data!;
        }
        
        // Search method for searching messages in dialogs.
        public async Task<SearchModel> SearchInDialog(string query, string id)
        {
            return (await GetAsync<SearchModel>($"/search/{id}?query={query}")).Data!;
        }
    }
}
