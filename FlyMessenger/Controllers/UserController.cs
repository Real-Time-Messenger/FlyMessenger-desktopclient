using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    class UserInEditName
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
    }

    class UserInEditEmail
    {
        public string Email { get; set; }
    }

    class UserInEditPhoto
    {
        public MultipartFormDataContent Photo { get; set; }
    }

    // Create dictionary of user id and user in edit


    public class UserController : HttpClientBase
    {
        public UserModel GetMyProfile()
        {
            return Get<UserModel>(Constants.ProfilesUrl + "/me");
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
        
        public UserModel EditMyProfilePhoto(MultipartFormDataContent photo)
        {
            return Put<UserModel, UserInEditPhoto>(Constants.ProfilesUrl + "/me/avatar", new UserInEditPhoto { Photo = photo });
        }
    }
}
