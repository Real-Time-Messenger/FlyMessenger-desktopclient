using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Documents;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using Microsoft.VisualBasic;
using RestSharp;
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
            return Get<DialogModel[]>(Constants.DialogsUrl + "/me");
        }
        
        public async Task<Collection<MessageModel>> LoadMoreMessages(string id, int skip)
        {
            return await GetAsync<Collection<MessageModel>>(Constants.DialogsUrl + $"/{id}/messages?skip={skip}");
        }
        
        public DialogModel CreateDialog(string id)
        {
            return Post<DialogModel, DialogInCreate>(Constants.DialogsUrl, new DialogInCreate { ToUserId = id });
        }

        public DialogModel EditDialog(string id, DialogInEdit data)
        {
            return Put<DialogModel, DialogInEdit>(Constants.DialogsUrl + $"/{id}", data);
        }

        public void DeleteDialog(string id)
        {
            Delete(Constants.DialogsUrl + $"/{id}");
        }
    }
}