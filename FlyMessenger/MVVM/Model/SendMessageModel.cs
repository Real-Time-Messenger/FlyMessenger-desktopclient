namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Send message model.
    /// </summary>
    public class SendMessageModel
    {
        public string type { get; set; }
        public FileModel? file { get; init; }
        public string dialogId { get; set; }
        public string? text { get; set; }
        public string recipientId { get; set; }
    }

    /// <summary>
    /// File model.
    /// </summary>
    public class FileModel
    {
        public string type { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public string data { get; set; }
        
    }
}
