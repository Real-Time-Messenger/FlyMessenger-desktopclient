namespace FlyMessenger.MVVM.Model
{
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
    
    public class DestroySessionModel
    {
        public string type { get; set; }
        public string sessionId { get; set; }
    }
}
