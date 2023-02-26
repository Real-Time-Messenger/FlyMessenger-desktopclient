using System;

namespace FlyMessenger.Core.Models
{
    public class SendMessageModel
    {
        public string type { get; set; }
        public FileModel? file { get; init; }
        public string dialogId { get; set; }
        public string? text { get; set; }
    }

    public class FileModel
    {
        public string type { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public string data { get; set; }
        
    }
}
