using System;

namespace FlyMessenger.Core.Models
{
    public class SendMessageModel
    {
        public string type { get; set; }
        public string? file { get; set; }
        public string dialogId { get; set; }
        public string text { get; set; }
    }
}
