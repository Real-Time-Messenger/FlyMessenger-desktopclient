namespace FlyMessenger.MVVM.Model
{
    public class SearchModel
    {
        public DialogModel[]? Dialogs { get; set; }
        public DialogModel[]? Messages { get; set; }
        public UserModel[]? Users { get; set; }
    }
    
    public class DialogInSearchModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhotoUrl { get; set; }
    }
}
