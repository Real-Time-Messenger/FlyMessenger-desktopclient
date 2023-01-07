using FlyMessenger.MVVM.Model;

namespace FlyMessenger.Controllers
{
    public static class ControllerBase
    {
        public static readonly DialogController DialogController = new DialogController();
        public static readonly SearchController SearchController = new SearchController();
        public static readonly UserController UserController = new UserController();
    }
}
