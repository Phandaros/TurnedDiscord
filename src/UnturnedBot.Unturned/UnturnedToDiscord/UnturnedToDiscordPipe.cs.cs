using Steamworks;
using System.IO;
using System.IO.Pipes;

namespace UnturnedBot.Unturned.UnturnedToDiscord
{
    public class UnturnedToDiscordPipe
    {
        public static StreamWriter writer;
        //public static StreamReader reader;

        public static void StartClient()
        {
            var client = new NamedPipeClientStream(".", "UnturnedToDiscord", PipeDirection.Out);
            client.Connect();

            writer = new StreamWriter(client)
            {
                AutoFlush = true
            };
        }

        public static void SendMessage(ulong channelID, string message)
        {
            SendToServer("sendmsgto|" + channelID + "|" + message);
        }
        public static void NotifyPlayerConnected(string displayName)
        {
            SendToServer("playerconnected|" + displayName);
        }
        public static void NotifyPlayerDisconnected(string displayName)
        {
            SendToServer("playerdisconnected|" + displayName);
        }
        public static void NotifyBuildDamaged(ulong owner)
        {
            SendToServer("builddamaged|" + owner);
        }
        public static void NotifyServerConnected()
        {
            SendToServer("connected");
        }
        public static void SendToServer(string str)
        {
            writer.WriteLine(str);
            writer.Flush();
        }
    }
}

