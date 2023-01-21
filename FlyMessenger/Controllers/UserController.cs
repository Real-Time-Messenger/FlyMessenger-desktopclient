using System.Collections.Generic;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    internal class UserInEditName
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
    }

    internal class UserInEditEmail
    {
        public string Email { get; set; }
    }

    internal class UserInEditLastActivity
    {
        public bool LastActivityMode { get; set; }
    }

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

        public UserModel EditMyProfileName(string name, string surname)
        {
            return Put<UserModel, UserInEditName>(
                Constants.ProfilesUrl + "/me",
                new UserInEditName
                {
                    FirstName = name,
                    LastName = surname
                }
            );
        }

        public UserModel EditMyProfileEmail(string email)
        {
            return Put<UserModel, UserInEditEmail>(Constants.ProfilesUrl + "/me", new UserInEditEmail { Email = email });
        }

        public UserModel EditMyProfilePhoto(byte[] photo)
        {
            return Put<UserModel>(Constants.ProfilesUrl + "/me/avatar", photo);
        }

        public UserModel EditMyLastActivity(bool lastActivity)
        {
            return Put<UserModel, UserInEditLastActivity>(
                Constants.ProfilesUrl + "/me",
                new UserInEditLastActivity { LastActivityMode = lastActivity }
            );
        }
    }
}
