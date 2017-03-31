using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord.UnturnedToDiscord
{
    class DiscordToUnturnedPipe
    {
        private static Dictionary<string, ResponseAction> Queue = new Dictionary<string, ResponseAction>();
        public delegate void ResponseAction(string response);
        private static List<ResponseAction> Actions = new List<ResponseAction>();

        public static StreamWriter writer;
        public static StreamReader reader;

        public static void Initialize()
        {
            Task.Run(() =>
            {
                var server = new NamedPipeServerStream("DiscordToUnturned", PipeDirection.InOut);
                server.WaitForConnection();
                Logger.Log("[DiscordToUnturned] connected, starting listening loop...");

                writer = new StreamWriter(server)
                {
                    AutoFlush = true
                };
                reader = new StreamReader(server);

                while (true)
                {
                    reader.BaseStream.Flush();
                    var command = reader.ReadLine();
                    HandleCommand(command);
                }
            });
        }

        private static void HandleCommand(string command)
        {
            switch(command.ToLower())
            {
                case "getcommand":
                    SendUpdate();
                    break;
                //Unturned reflete os comandos que eu mando por algum motivo, por isso ignoro
                case "getplayers":
                case "addstructures":
                case "checkstructures":
                    break;
                //
                default:
                    var act = Actions.FirstOrDefault();
                    if (act != null)
                    {
                        act.Invoke(command);
                        Actions.Remove(act);
                        Logger.Log("[DiscordToUnturned] ActionResponse: " + command);
                    }
                    else
                        Logger.Log("[DiscordToUnturned] CommandNotFound: " + command);
                    break;
            }
        }

        public static void SendUpdate()
        {
            var fromQueue = Queue.FirstOrDefault();
            if (fromQueue.Key != null)
            {
                writer.WriteLine(fromQueue.Key);

                var act = fromQueue.Value;
                if (act != null)
                    Actions.Add(act);

                Queue.Remove(fromQueue.Key);
            }
            else
                writer.WriteLine("empty");
        }
        public static void AddToQueue(string str, ResponseAction act = null)
        {
            Queue.Add(str, act);
        }
    }
}
