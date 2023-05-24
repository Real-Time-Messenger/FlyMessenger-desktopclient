using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using RestSharp;

namespace FlyMessenger.Controllers
{
    /// <summary>
    /// User controller.
    /// </summary>
    public class UserController : HttpClientBase
    {
        // Get user profile.
        public UserModel GetMyProfile()
        {
            var user = Get<UserModel>(Constants.ProfilesUrl + "/me").Data!;
            return user;
        }

        // Get user sessions.
        public SessionsModel[] GetMySessions()
        {
            return Get<SessionsModel[]>(Constants.ProfilesUrl + "/me/sessions").Data!;
        }

        // Edit user first name and last name.
        public async Task EditMyProfileName(string? name, string? surname)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new
                {
                    FirstName = name,
                    LastName = surname
                }
            );
        }

        // Edit user email.
        public async Task EditMyProfileEmail(string? email)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new
                {
                    Email = email
                }
            );
        }

        // Edit user photo.
        public async Task EditMyProfilePhoto(byte[] photo)
        {
            await PutAsync<UserModel>(Constants.ProfilesUrl + "/me/avatar", photo);
        }

        // Edit user last activity.
        public async Task EditMyLastActivity(bool lastActivity)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { LastActivityMode = lastActivity }
            );
        }

        // Enable/disable notifications.
        public async Task EditMyChatsNotifications(bool chatsNotifications)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { ChatsNotificationsEnabled = chatsNotifications }
            );
        }

        // Enable/disable notification sound.
        public async Task EditMyChatsSound(bool chatsSound)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { ChatsSoundEnabled = chatsSound }
            );
        }

        // Enable/disable 2FA.
        public async Task EditMyTwoFactorAuth(object twoFactor)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { TwoFactorAuthEnabled = twoFactor }
            );
        }

        // Enable/disable auto start on system start.
        public async Task EditMyAutoStart(bool autoStart)
        {
            await PutAsync<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { AutoStartEnabled = autoStart }
            );
        }
        
        // Logout.
        public void Logout()
        {
            Post(Constants.AuthUrl + "/logout");
        }

        // Delete user.
        public async Task Delete()
        {
            await DeleteAsync(Constants.ProfilesUrl + "/me");
        }

        // Block/unblock user.
        public async Task<BlackListResponseModel> BlockOrUnblockUser(string userId)
        {
            return await PostAsync<BlackListResponseModel, object>(
                Constants.ProfilesUrl + "/blacklist",
                new { BlacklistedUserId = userId }
            ).ContinueWith(task => task.Result.Data!);
        }

        // Login user.
        public async Task<RestResponse<LoginModel>> Login(string username, string password)
        {
            return await PostAsync<LoginModel, object>(
                Constants.AuthUrl + "/login",
                new { Username = username, Password = password }
            );
        }

        // Register user.
        public async Task<RestResponse<RegisterModel>> Register(string username, string email, string password, string passwordConfirm)
        {
            return await PostAsync<RegisterModel, object>(
                Constants.AuthUrl + "/signup",
                new { Username = username, Email = email, Password = password, PasswordConfirm = passwordConfirm }
            );
        }
        
        // Reset password.
        public async Task<RestResponse<CallResetPasswordModel>> CallResetPassword(string email)
        {
            return await PostAsync<CallResetPasswordModel, object>(
                Constants.AuthUrl + "/call-reset-password",
                new { Email = email }
            );
        }
        
        // Two factor authentication.
        public async Task<RestResponse<TwoFactorModel>> TwoFactorAuthenticate(string code)
        {
            return await PostAsync<TwoFactorModel, object>(
                Constants.AuthUrl + "/two-factor",
                new { Code = code }
            );
        }
        
        // Confirm new device.
        public async Task<RestResponse<TwoFactorModel>> ConfirmNewDevice(string code)
        {
            return await PostAsync<TwoFactorModel, object>(
                Constants.AuthUrl + "/new-device",
                new { Code = code }
            );
        }
    }
}
