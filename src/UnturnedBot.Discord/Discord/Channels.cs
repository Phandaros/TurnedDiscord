using Discord;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.Discord
{
    static class Channels
    {
        internal static IUser owner;
        internal static IGuild mainGuild;

        internal static IMessageChannel debug;
        internal static IMessageChannel general;
        internal static IMessageChannel modLog;
        internal static IMessageChannel playerUpdates;

        internal static async Task StartAsync()
        {
            owner = DiscordBot.client.GetUser(229740955463450624);
            mainGuild = DiscordBot.client.GetGuild(290153260076236800);

            debug = await mainGuild.GetChannelAsync(290181727241109504) as IMessageChannel;
            general = await mainGuild.GetChannelAsync(290153260076236800) as IMessageChannel;
            modLog = await mainGuild.GetChannelAsync(290153617233674241) as IMessageChannel;
            playerUpdates = await mainGuild.GetChannelAsync(290885633772224512) as IMessageChannel;
        }
    }
}
