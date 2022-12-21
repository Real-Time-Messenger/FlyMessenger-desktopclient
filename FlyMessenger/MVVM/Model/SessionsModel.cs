using System;

namespace FlyMessenger.MVVM.Model
{
    public abstract class SessionTypes
    {
        public const string Desktop = "desktop";
        public const string Web = "web";
    }
    
    public class SessionsModel
    {
        public string Id { get; set; }
        public string IpAddress { get; set; }
        public SessionTypes Type { get; set; }
        public string Location { get; set; }
        public string Label { get; set; }
        public string CreatedAt { get; set; }
    }
}
