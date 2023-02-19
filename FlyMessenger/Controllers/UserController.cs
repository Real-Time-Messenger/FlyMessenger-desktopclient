using System.Collections.Generic;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    public class UserController : HttpClientBase
    {
        public UserModel GetMyProfile()
        {
            return Get<UserModel>(Constants.ProfilesUrl + "/me");
        }

        public IEnumerable<SessionsModel> GetMySessions()
        {
            return Get<SessionsModel[]>(Constants.ProfilesUrl + "/me/sessions");
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

        public BlackListResponseModel BlockOrUnblockUser(string userId)
        {
            return Post<BlackListResponseModel, object>(
                Constants.ProfilesUrl + "/blacklist",
                new { BlacklistedUserId = userId }
            );
        }
    }
}
