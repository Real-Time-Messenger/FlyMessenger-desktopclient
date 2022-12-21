namespace FlyMessenger.MVVM.Model
{
    public class Sender
    {
        public string Id { get; set; }
        public string PhotoUrl { get; set; }
    }
    
    public class LastMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string File { get; set; }
        public string SentAt { get; set; }
        public bool IsRead { get; set; }
        public Sender Sender { get; set; }
    }
    
    public class MessageModel
    {
        public string Id { get; set; }
        public Sender Sender { get; set; }
        public string? Text { get; set; }
        public string? File { get; set; }
        public bool IsRead { get; set; }
        public string SentAt { get; set; }
    }
    
    public class DialogModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public UserModel User { get; set; }
        public string[] Images { get; set; }
        public DialogModel[] Messages { get; set; }
        public LastMessage? LastMessage { get; set; }
        public int UnreadMessages { get; set; }
        public bool IsPinned { get; set; }
        public bool IsSoundEnabled { get; set; }
        public bool IsNotificationsEnabled { get; set; }
        public bool IsMeBlocked { get; set; }
    }
}
