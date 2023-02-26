using System.Threading.Tasks;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using RestSharp;

namespace FlyMessenger.Controllers
{
    public class UserController : HttpClientBase
    {
        public UserModel GetMyProfile()
        {
            return Get<UserModel>(Constants.ProfilesUrl + "/me").Data!;
        }

        public SessionsModel[] GetMySessions()
        {
            return Get<SessionsModel[]>(Constants.ProfilesUrl + "/me/sessions").Data!;
        }

        public void EditMyProfileName(string? name, string? surname)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new
                {
                    FirstName = name,
                    LastName = surname
                }
            );
        }

        public void EditMyProfileEmail(string? email)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new
                {
                    Email = email
                }
            );
        }

        public void EditMyProfilePhoto(byte[] photo)
        {
            Put<UserModel>(Constants.ProfilesUrl + "/me/avatar", photo);
        }

        public void EditMyLastActivity(bool lastActivity)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { LastActivityMode = lastActivity }
            );
        }

        public void EditMyChatsNotifications(bool chatsNotifications)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { ChatsNotificationsEnabled = chatsNotifications }
            );
        }

        public void EditMyChatsSound(bool chatsSound)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { ChatsSoundEnabled = chatsSound }
            );
        }

        public void EditMyTwoFactorAuth(object twoFactor)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { TwoFactorAuthEnabled = twoFactor }
            );
        }

        public void EditMyAutoStart(bool autoStart)
        {
            Put<UserModel, object>(
                Constants.ProfilesUrl + "/me",
                new { AutoStartEnabled = autoStart }
            );
        }
        
        public void Logout()
        {
            Post(Constants.AuthUrl + "/logout");
        }

        public void Delete()
        {
            Delete(Constants.ProfilesUrl + "/me");
        }

        public BlackListResponseModel BlockOrUnblockUser(string userId)
        {
            return Post<BlackListResponseModel, object>(
                Constants.ProfilesUrl + "/blacklist",
                new { BlacklistedUserId = userId }
            ).Data!;
        }

        public async Task<RestResponse<LoginModel>> Login(string username, string password)
        {
            return await PostAsync<LoginModel, object>(
                Constants.AuthUrl + "/login",
                new { Username = username, Password = password }
            );
        }

        public async Task<RestResponse<RegisterModel>> Register(string username, string email, string password, string passwordConfirm)
        {
            return await PostAsync<RegisterModel, object>(
                Constants.AuthUrl + "/signup",
                new { Username = username, Email = email, Password = password, PasswordConfirm = passwordConfirm }
            );
        }
        
        public async Task<RestResponse<CallResetPasswordModel>> CallResetPassword(string email)
        {
            return await PostAsync<CallResetPasswordModel, object>(
                Constants.AuthUrl + "/call-reset-password",
                new { Email = email }
            );
        }
        
        public async Task<RestResponse<TwoFactorModel>> TwoFactorAuthenticate(string code)
        {
            return await PostAsync<TwoFactorModel, object>(
                Constants.AuthUrl + "/two-factor",
                new { Code = code }
            );
        }
        
        public async Task<RestResponse<TwoFactorModel>> ConfirmNewDevice(string code)
        {
            return await PostAsync<TwoFactorModel, object>(
                Constants.AuthUrl + "/new-device",
                new { Code = code }
            );
        }
    }
}
