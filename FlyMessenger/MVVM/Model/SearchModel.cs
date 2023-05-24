namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Search model.
    /// </summary>
    public class SearchModel
    {
        public DialogModel[]? Dialogs { get; set; }
        public DialogModel[]? Messages { get; set; }
        public DialogModel[]? Users { get; set; }
    }
    
    /// <summary>
    /// Dialog in search model.
    /// </summary>
    public class DialogInSearchModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhotoUrl { get; set; }
    }
}
