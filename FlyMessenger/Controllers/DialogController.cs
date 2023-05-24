using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using FlyMessenger.MVVM.ViewModels;
using Constants = FlyMessenger.HTTP.Constants;

namespace FlyMessenger.Controllers
{
    // Model for creating dialog
    class DialogInCreate
    {
        public string ToUserId { get; set; }
    }

    // Model for editing dialog
    public class DialogInEdit
    {
        public bool? IsPinned { get; set; }
        public bool? IsSoundEnabled { get; set; }
        public bool? IsNotificationsEnabled { get; set; }
    }

    /// <summary>
    /// Controller for dialogs
    /// </summary>
    public class DialogController : HttpClientBase
    {
        // Get all user dialogs
        public DialogModel[] GetDialogs()
        {
            return Get<DialogModel[]>(Constants.DialogsUrl + "/me").Data!;
        }

        // Get dialog from id with skip
        // Method is used for loading more messages when scrolling
        public async Task<Collection<MessageModel>> LoadMoreMessages(string id, int skip)
        {
            return (await GetAsync<Collection<MessageModel>>(Constants.DialogsUrl + $"/{id}/messages?skip={skip}")).Data!;
        }

        // Edit dialog settings
        public async Task<DialogModel> EditDialog(string id, DialogInEdit data)
        {
            return await PutAsync<DialogModel, DialogInEdit>(Constants.DialogsUrl + $"/{id}", data)
                .ContinueWith(task => task.Result.Data!);
        }

        // Delete dialog
        public async Task DeleteDialog(string id)
        {
            await DeleteAsync(Constants.DialogsUrl + $"/{id}");
        }
    }
}
