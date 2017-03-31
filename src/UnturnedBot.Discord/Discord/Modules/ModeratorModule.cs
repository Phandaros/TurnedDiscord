using Discord;
using UnturnedBot.Discord.Discord.Preconditions;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using UnturnedBot.Discord.Discord.Utils;
using UnturnedBot.Discord.Utils;

//using RequireUserPermission = UnturnedBot.Discord.Discord.Preconditions.RequireUserChannelPermissionAttribute;

namespace UnturnedBot.Discord.Discord.Modules
{
    public class ModeratorModule : ModuleBase<ICommandContext>
    {
        [Command("clear"), Summary("Deleta o número de mensagems especificado"), RequireUserChannelPermission(ChannelPermission.ManageMessages)]
        public async Task ClearAsync(int numberOfMessages)
        {
            if (numberOfMessages > 99)
            {
                var msg = await ReplyAsync("O número de mensagems é muito grande!");
                await msg.DeleteAsync();
                await Context.Message.DeleteAsync();
            }
            else
            {
                var messages = await Context.Channel.GetMessagesAsync(numberOfMessages + 1).Flatten();
                await Context.Channel.DeleteMessagesAsync(messages);
            }
        }

        [Command("clear"), Summary("Deleta o número de mensagems especificado"), RequireUserChannelPermission(ChannelPermission.ManageMessages)]
        public async Task ClearAsync(IUser user)
        {
            await Context.Message.DeleteAsync();
            var messages = await Context.Channel.GetMessagesAsync(1000).Flatten();
            var userMessages = messages.Where(x => x.Author == user);
            await Context.Channel.DeleteMessagesAsync(userMessages);
        }

        [Command("kick"), Summary("Kika o usuário"), RequireUserGuildPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(IGuildUser user, [Remainder] string reason)
        {
            var caseID = int.Parse(Settings.GetValue("Cases"));

            var moderator = Context.User;

            var embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder().WithName(user.Username + "#" + user.Discriminator).WithIconUrl(user.GetAvatarUrl()))
                .WithColor(Colors.Yellow)
                .WithTitle("Usuário kikado")
                .WithDescription(Format.Bold("Reason: ") + reason + "\n" +
                                 Format.Bold("Moderador responsável: ") + moderator.Mention)
                .WithFooter(new EmbedFooterBuilder().WithText("CaseID: " + caseID));

            var msg = await Channels.modLog.SendMessageAsync("", embed: embed);

            Settings.AddOrUpdate("Cases", (caseID + 1).ToString());

            await user.KickAsync();
        }
        [Command("ban"), Summary("Bane o usuário"), RequireUserGuildPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(IGuildUser user, [Remainder] string reason)
        {
            var caseID = Settings.GetValue("Cases");

            var moderator = Context.User;

            var embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder().WithName(user.Username + "#" + user.Discriminator).WithIconUrl(user.GetAvatarUrl()))
                .WithColor(Colors.Red)
                .WithTitle("Usuário banido")
                .WithDescription(Format.Bold("Reason: ") + reason + "\n" +
                                 Format.Bold("Moderador responsável: ") + moderator.Mention + "\n" +
                                 Format.Bold("UserID: ") + user.Id)
                .WithFooter(new EmbedFooterBuilder().WithText("CaseID: " + caseID));

            var msg = await Channels.modLog.SendMessageAsync("", embed: embed);

            Settings.AddOrUpdate("Cases", (int.Parse(caseID) + 1).ToString());

            await Context.Guild.AddBanAsync(user);
        }
    }
}
