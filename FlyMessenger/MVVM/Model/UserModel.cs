using System;

namespace FlyMessenger.MVVM.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public bool IsActivate { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsOnline { get; set; }
        public string? LastActivity { get; set; }
        public string CreatedAt { get; set; }

        public SettingsModel Settings { get; set; }
        public SessionsModel[] Sessions { get; set; }
        
        public BlackListModel[] BlackList { get; set; }
    }
}
