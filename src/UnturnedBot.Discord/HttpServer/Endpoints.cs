using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnturnedBot.Discord.HttpServer.Json;

namespace UnturnedBot.Discord.HttpServer
{
    class EndPoints
    {
        [Enpoint("getname")]
        public static string GetName(string name)
        {
            return "You entered: " + name;
        }

        [Enpoint("SendMessageTo")]
        public static SendMessageResult SendMessage(ulong channelID, string message)
        {
            if (!Discord.Utils.Helper.TrySendMessageToChannel(channelID, message).Result)
            {
                return new SendMessageResult(SendMessageResultType.ChannelNotFound);
            }
            return new SendMessageResult(SendMessageResultType.MessageSent);
        }
    }
}