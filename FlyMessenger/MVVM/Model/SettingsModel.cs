namespace FlyMessenger.MVVM.Model
{
    public class SettingsModel
    {
        public string Theme { get; set; }
        public string Language { get; set; }
        public string NotificationPosition { get; set; }
        public bool ChatsNotificationsEnabled { get; set; }
        public bool ChatsSoundEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LastActivityMode { get; set; }
        public bool IsDisplayNameVisible { get; set; }
        public bool IsEmailVisible { get; set; }
    }
}
