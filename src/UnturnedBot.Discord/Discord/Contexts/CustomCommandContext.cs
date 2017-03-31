using Discord.Commands;
using Discord;

namespace UnturnedBot.Discord.Discord.Contexts
{
    class CustomCommandContext : ICommandContext
    {
        internal static IUserMessage dummyMsg = null;
        IDiscordClient ICommandContext.Client => DiscordBot.client;

        IGuild ICommandContext.Guild => Channels.mainGuild;

        IMessageChannel ICommandContext.Channel => Channels.debug;

        IUser ICommandContext.User => Channels.owner;

        IUserMessage ICommandContext.Message => dummyMsg;
    }
}
