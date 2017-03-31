using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord.Discord.Modules
{
    [Name("Geral")]
    public class PublicModule : ModuleBase<ICommandContext>
    {
        [Command("invitelink"), Summary("Mostra o link permanente de invite para sua guilda")]
        public async Task InvitelinkAsync()
        {
            var invites = await Context.Guild.GetInvitesAsync();
            var mainInvite = invites.FirstOrDefault(x => !x.MaxAge.HasValue && x.ChannelId == Context.Guild.DefaultChannelId);

            if (mainInvite == null)
            {
                Logger.Log("Invite not found, " + invites.Count);
                var defaultChannel = await Context.Guild.GetDefaultChannelAsync();
                var newInvite = await defaultChannel.CreateInviteAsync(null, null);
                await ReplyAsync(newInvite.Url);
            }
            else
                await ReplyAsync(mainInvite.Url);
        }

        [Command("leave"), Summary("Faz o bot sair dessa guilda"), Preconditions.RequireUserGuildPermission(GuildPermission.Administrator)]
        public async Task LeaveAsync()
        {
            if (Context.Guild == null) { await ReplyAsync("Esse comando só pode ser executado em um server!"); return; }
            await ReplyAsync("~Leaving~");
            await Context.Guild.LeaveAsync();
        }

        [Command("info"), Summary("Informações do bot.")]
        public async Task InfoAsync()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            DiscordSocketClient client = Context.Client as DiscordSocketClient;
            await ReplyAsync(
                $"{Format.Bold("Info")}\n" +
                $"- Author: {application.Owner.Username} (ID {application.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                $"- Uptime: {GetUptime()}\n\n" +
                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {GetHeapSize()} MB\n" +
                $"- Guilds: {client.Guilds.Count}\n" +
                $"- Channels: {client.Guilds.Sum(g => g.Channels.Count)}\n" +
                $"- Users: {client.Guilds.Sum(g => g.Users.Count)}"
            );
        }

        [Command("exit"), Summary("Fecha o bot")]
        public async Task ExitAsync()
        {
            if (Context.Guild.Id == 230100292375543809)
            {
                await ReplyAsync("<:heyguys:241310983983857664>");
            }
            await DiscordBot.client.StopAsync();
            Environment.Exit(0);
        }

        [Command("login"), Summary("Troca o usúario do bot")]
        public async Task LoginAsync(string username)
        {
            var user = TokenUtils.Tokens.FirstOrDefault(x => x.Key.Equals(username, StringComparison.InvariantCultureIgnoreCase));
            if (user.Key != null)
            {
                await LoginAsync(user.Value.token, user.Value.tokenType);
                return;
            }
            await ReplyAsync(":grey_question:");
        }
        [Command("login"), Summary("Troca o usúario do bot")]
        public async Task LoginAsync(string token, TokenType tokenType)
        {
            await DiscordBot.client.StopAsync();
            await Task.Delay(3000);

            await DiscordBot.client.LoginAsync(tokenType, token);
            await DiscordBot.client.StartAsync();

            Task Connected()
            {
                DiscordBot.client.Connected -= Connected;

                Logger.Log("[Discord] [Login] Now logged in as " + DiscordBot.client.CurrentUser.Username + ".");
                return Task.CompletedTask;
            }
            DiscordBot.client.Connected += Connected;
        }

        [Command("setnick"), RequireOwner()]
        public async Task ChangeNickAsync([Remainder] string nick = "")
        {
            await (await Context.Guild.GetCurrentUserAsync()).ModifyAsync(x => x.Nickname = nick);
        }
        [Command("setgame"), RequireOwner()]
        public async Task ChangeGameAsync([Remainder] string game = "")
        {
            await DiscordBot.client.SetGameAsync(game);
        }

        static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
