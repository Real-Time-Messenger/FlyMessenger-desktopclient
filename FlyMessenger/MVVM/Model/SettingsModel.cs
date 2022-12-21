namespace FlyMessenger.MVVM.Model
{
    public abstract class Languages
    {
        public static readonly string EN = "en";
        public static readonly string RU = "ru";
        public static readonly string ET = "et";
    }
    
    public abstract class Themes
    {
        public static readonly string Light = "light";
        public static readonly string Dark = "dark";
    }
    
    public abstract class NotificationPositions
    {
        public static readonly string TopRight = "top-right";
        public static readonly string TopLeft = "top-left";
        public static readonly string BottomRight = "bottom-right";
        public static readonly string BottomLeft = "bottom-left";
    }
    
    public class SettingsModel
    {
        public Themes Theme { get; set; }
        public Languages Language { get; set; }
        public NotificationPositions NotificationPosition { get; set; }
        public bool ChatsNotificationsEnabled { get; set; }
        public bool ChatsSoundEnabled { get; set; }
        public bool TwoFactorAuth { get; set; }
        public bool LastActivityMode { get; set; }
        public bool AllowRunOnStartup { get; set; }
    }
}
