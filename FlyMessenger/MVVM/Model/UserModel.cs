using System.Collections.ObjectModel;

namespace FlyMessenger.MVVM.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        
        public bool IsActive { get; set; }
        public string PhotoUrl { get; set; }
        public bool? IsOnline { get; set; }
        public string? LastActivity { get; set; }
        public string CreatedAt { get; set; }

        public SettingsModel Settings { get; set; }
        public ObservableCollection<SessionsModel?>? Sessions { get; set; } = new ObservableCollection<SessionsModel?>();
        
        public ObservableCollection<BlackListModel> BlackList { get; set; } = new ObservableCollection<BlackListModel>();
    }
    
    public class LoginModel
    {
        public Details[] Details { get; set; }
        public string? Translation { get; set; }
        public string? Token { get; set; }
        public string? Event { get; set; }
    }
    
    public class RegisterModel
    {
        public Details[] Details { get; set; }
        public string? Event { get; set; }
    }
    
    public class CallResetPasswordModel
    {
        public Details[] Details { get; set; }
        public string? Event { get; set; }
        public string? Translation { get; set; }
    }
    
    public class TwoFactorModel
    {
        public Details[] Details { get; set; }
        public string? Token { get; set; }
        public string? Translation { get; set; }
    }
}
