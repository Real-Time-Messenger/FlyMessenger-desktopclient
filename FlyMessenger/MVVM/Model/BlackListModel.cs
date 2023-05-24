using System.Collections.ObjectModel;

namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Black list model.
    /// </summary>
    public class BlackListModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
    }
    
    /// <summary>
    /// Black list response model.
    /// </summary>
    public class BlackListResponseModel
    {
        public string UserId { get; set; }
        public bool IsBlocked { get; set; }
        public ObservableCollection<BlackListModel> BlackList { get; set; } = new ObservableCollection<BlackListModel>();
    }
}
