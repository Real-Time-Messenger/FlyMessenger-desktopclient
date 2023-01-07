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
    }
}
