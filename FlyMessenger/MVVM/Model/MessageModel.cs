using System;
using System.Drawing;

namespace FlyMessenger.MVVM.Model
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string MessageOwner { get; set; }
        public string MessageOwnerImage { get; set; }
        public string Message { get; set; }
        public string MessagesCount { get; set; }
        public bool MessageAnchor { get; set; }
        public string MessageDateTime { get; set; }
        public bool MessageStatus { get; set; }
        public bool MessageNotification { get; set; }
    }
}
