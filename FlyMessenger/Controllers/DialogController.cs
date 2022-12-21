using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using RestSharp;

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
        public bool? IsNotificationEnabled { get; set; }
    }
    
    public class DialogController : HttpClientBase
    {
        public IEnumerable<DialogModel> GetDialogs()
        {
            return Get<DialogModel[]>("/dialogs/me");
        }

        public DialogModel CreateDialog(string id)
        {
            return Post<DialogModel, DialogInCreate>("/dialogs", new DialogInCreate { ToUserId = id });
        }

        public DialogModel EditDialog(string id, DialogInEdit data)
        {
            return Put<DialogModel, DialogInEdit>($"/dialogs/{id}", data);
        }

        public void DeleteDialog(string id)
        {
            Delete($"/dialogs/{id}");
        }
    }
}