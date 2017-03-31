using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using UnturnedBot.Discord.Utils;
using System.Linq;

namespace UnturnedBot.Discord.Discord
{
    public class DiscordBot
    {
        internal static DiscordSocketClient client;
        internal static CommandHandler handler;
        internal static DependencyMap _map = new DependencyMap();

        internal static async void Start()
        {
            TokenUtils.SetDefaults();
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });
            client.Log += (l)
                => Task.Run(()
                => Logger.Log($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));
            handler = new CommandHandler(new CommandService(new CommandServiceConfig() { DefaultRunMode = RunMode.Async }));
            _map.Add(client);

            var token = TokenUtils.Tokens.FirstOrDefault().Value;

            await client.LoginAsync(token.tokenType, token.token);
            await client.StartAsync();

            client.Connected += Client_Connected;

            MainWindow.UpdateBotStatus();
            client.Connected += () => { MainWindow.UpdateBotStatus(); return Task.CompletedTask; };
            client.Ready += () => { MainWindow.UpdateBotStatus(); return Task.CompletedTask; };
            client.Disconnected += (ex) => { MainWindow.UpdateBotStatus(); return Task.CompletedTask; };
        }

        private static async Task Client_Connected()
        {
            await Task.Delay(1000);

            Logger.Log("[Discord] Client ready.");
            await Channels.StartAsync();
        }
    }
}
