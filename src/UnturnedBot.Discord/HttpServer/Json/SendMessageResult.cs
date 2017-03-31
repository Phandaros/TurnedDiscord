using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.HttpServer.Json
{
    class SendMessageResult
    {
        public string Message { get; private set; }
        public SendMessageResult(SendMessageResultType result)
        {
            Message = result.ToString();
        }
    }
    enum SendMessageResultType
    {
        MessageSent, ChannelNotFound
    }
}
