namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Settings model.
    /// </summary>
    public class SettingsModel
    {
        public string Theme { get; set; }
        public string Language { get; set; }
        public bool ChatsNotificationsEnabled { get; set; }
        public bool ChatsSoundEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LastActivityMode { get; set; }
        public bool AllowRunOnStartup { get; set; }
    }
}
