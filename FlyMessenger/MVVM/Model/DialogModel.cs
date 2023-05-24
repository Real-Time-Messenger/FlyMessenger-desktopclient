using System.Collections.ObjectModel;
using FlyMessenger.Core;

namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Sender model.
    /// </summary>
    public class Sender
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
    }

    /// <summary>
    /// Message model.
    /// </summary>
    public class MessageModel
    {
        public string Id { get; set; }
        public string DialogId { get; set; }
        public Sender Sender { get; set; }
        public string? Text { get; set; }
        public string? File { get; set; }
        public bool IsRead { get; set; }
        public string SentAt { get; set; }
        public bool? NewDate { get; set; }
        public bool? IsMyMessage { get; set; }
        public bool? IsMyPhotoVisible { get; set; }
        public bool? IsCompanionPhotoVisible { get; set; }
    }

    /// <summary>
    /// User in dialog model.
    /// </summary>
    public class UserInDialogModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsOnline { get; set; }
        public string LastActivity { get; set; }
        public bool IsTyping { get; set; }
        public bool IsBlocked { get; set; }
    }

    /// <summary>
    /// Dialog model.
    /// </summary>
    public class DialogModel : ObservableObject
    {
        public string Id { get; set; }
        public UserInDialogModel User { get; set; }
        public Collection<MessageModel> Messages { get; set; } = new Collection<MessageModel>();
        
        private MessageModel? _lastMessage;
        
        public MessageModel? LastMessage
        {
            get => _lastMessage;
            
            // If last message is null, set default message.
            set
            {
                if (value == null)
                {
                    _lastMessage = new MessageModel
                    {
                        Text = Resources.Languages.lang.send_first_message,
                        IsMyMessage = false,
                        IsMyPhotoVisible = false,
                        IsCompanionPhotoVisible = false
                    };
                    OnPropertyChanged();
                    return;
                }
                _lastMessage = value;
                OnPropertyChanged();
            }
        }
        
        public string? Typing { get; set; }
        public int UnreadMessages { get; set; }
        public bool IsPinned { get; set; }
        public bool IsSoundEnabled { get; set; }
        public bool IsNotificationsEnabled { get; set; }
        public bool IsMeBlocked { get; set; }
    }
}
