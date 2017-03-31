using Discord;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.Discord.Utils
{
    static class Helper
    {
        public static EmbedBuilder BuildEmbed(string title, string description, Color color)
        {
            return new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color);
        }

        public static async Task<bool> TrySendMessageToChannel(ulong chnID, string message)
        {
            var channel = DiscordBot.client.GetChannel(chnID) as IMessageChannel;
            if (channel == null) return false;

            await channel.SendMessageAsync(message);
            return true;
        }
    }
}
