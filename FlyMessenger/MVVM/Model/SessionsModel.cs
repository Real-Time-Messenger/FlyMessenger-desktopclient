namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Sessions model.
    /// </summary>
    public class SessionsModel
    {
        public string Id { get; set; }
        public string IpAddress { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string CreatedAt { get; set; }
        public bool? Current { get; set; }
    }
    
    /// <summary>
    /// Sessions response model.
    /// </summary>
    public class DestroySessionModel
    {
        public string type { get; set; }
        public string sessionId { get; set; }
    }
}
