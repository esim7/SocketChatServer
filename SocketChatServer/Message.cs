using System;
using System.Collections.Generic;
using System.Text;

namespace SocketChatServer
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string From { get; set; }
        public DateTime SendTime { get; set; }
        public string Text { get; set; }
    }
}
