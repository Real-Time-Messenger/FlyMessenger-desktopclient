namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Read message model.
    /// </summary>
    public class ReadMessageModel
    {
        public string type { get; set; }
        public string dialogId { get; set; }
        public string messageId { get; set; }
    }
}
