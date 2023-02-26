using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using Constants = FlyMessenger.HTTP.Constants;

namespace FlyMessenger.Controllers
{
    class DialogInCreate
    {
        public string ToUserId { get; set; }
    }

    public class DialogInEdit
    {
        public bool? IsPinned { get; set; }
        public bool? IsSoundEnabled { get; set; }
        public bool? IsNotificationsEnabled { get; set; }
    }
    
    public class DialogController : HttpClientBase
    {
        public DialogModel[] GetDialogs()
        {
            return Get<DialogModel[]>(Constants.DialogsUrl + "/me").Data!;
        }
        
        public async Task<Collection<MessageModel>> LoadMoreMessages(string id, int skip)
        {
            return (await GetAsync<Collection<MessageModel>>(Constants.DialogsUrl + $"/{id}/messages?skip={skip}")).Data!;
        }
        
        public DialogModel CreateDialog(string id)
        {
            return Post<DialogModel, DialogInCreate>(Constants.DialogsUrl, new DialogInCreate { ToUserId = id }).Data!;
        }

        public DialogModel EditDialog(string id, DialogInEdit data)
        {
            return Put<DialogModel, DialogInEdit>(Constants.DialogsUrl + $"/{id}", data).Data!;
        }

        public void DeleteDialog(string id)
        {
            Delete(Constants.DialogsUrl + $"/{id}");
        }
    }
}