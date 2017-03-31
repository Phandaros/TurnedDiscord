using Discord;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnturnedBot.Discord.Discord;
using UnturnedBot.Discord.Discord.Contexts;
using UnturnedBot.Discord.Discord.Utils;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord.UnturnedToDiscord
{
    class UnturnedToDiscordPipe
    {
        [XmlArrayItem]
        public static StreamReader reader;

        public static void StartServer()
        {
            Task.Run(async () =>
            {
                var server = new NamedPipeServerStream("UnturnedToDiscord", PipeDirection.In);
                server.WaitForConnection();
                Logger.Log("[UnturnedToDiscord] connected, starting listening loop...");

                reader = new StreamReader(server);

                while (true)
                {
                    await HandleInputAsync(await reader.ReadLineAsync());
                }
            });
        }

        private static async Task HandleInputAsync(string input)
        {
            if (input.StartsWith("<Discord>"))
            {
                Logger.Log("[UnturnedToDiscord] Client devolvendo as mensagems, ignorando...");
                return;
            }

            var args = input.Split('|');

            Logger.Log("[UnturnedToDiscord] Client: " + input);

            switch (args[0].ToLower())
            {
                case "sendmsgto":
                    var channelID = ulong.Parse(args[1]);
                    var channel = await Channels.mainGuild.GetChannelAsync(channelID) as IMessageChannel;
                    if (channel == null) break;
                    await channel.SendMessageAsync(args[2]);
                    break;
                case "handlecommand":
                    await DiscordBot.handler.HandleCommandAsync(new CustomCommandContext(), args[1]);
                    break;
                case "connected":
                    await Channels.debug.SendMessageAsync("", embed: new EmbedBuilder().WithTitle("Server ready!").WithColor(Colors.Green));
                    break;
                case "playerconnected":
                    Players.OnlinePlayers.Add(args[1]);
                    await Channels.playerUpdates.SendMessageAsync("", embed: Helper.BuildEmbed("Player online", args[1] + " conectou-se ao servidor.", Colors.Green));
                    break;
                case "playerdisconnected":
                    Players.OnlinePlayers.Remove(args[1]);
                    await Channels.playerUpdates.SendMessageAsync("", embed: Helper.BuildEmbed("Player offline", args[1] + " desconectou-se do servidor.", Colors.Red));
                    break;
                case "builddamaged":
                    await Channels.debug.SendMessageAsync("Build from " + args[1] + " damaged!");
                    break;
                default:
                    Logger.Log("[UnturnedToDiscord]  Client sent message " + input + " but there's no command for that");
                    break;
            }
        }
    }
}
