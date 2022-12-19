using System.Collections.ObjectModel;
using System.Linq;

namespace FlyMessenger.MVVM.Model
{
    public class ContactModel
    {
        public string Username { get; set; }
        public string ImageSource { get; set; }
        public string OnlineStatus { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public string LastMessage
        {
            get => Messages.Last().Message;
        }
    }
}
