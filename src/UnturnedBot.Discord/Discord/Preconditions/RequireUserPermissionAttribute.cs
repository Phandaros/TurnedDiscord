using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using UnturnedBot.Discord.Discord.Contexts;

namespace UnturnedBot.Discord.Discord.Preconditions
{
    class RequireUserChannelPermissionAttribute : PreconditionAttribute
    {
        ChannelPermission cPerm;
        public RequireUserChannelPermissionAttribute(ChannelPermission perm) => cPerm = perm;

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (context is CustomCommandContext)
                return Task.FromResult(PreconditionResult.FromSuccess());


            var guildUser = context.User as IGuildUser;
            if (guildUser.GetPermissions(context.Channel as IGuildChannel).Has(cPerm))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Você não tem as permissões necessárias para executar esse comando."));
        }
    }
    class RequireUserGuildPermissionAttribute : PreconditionAttribute
    {
        GuildPermission gPerm;
        public RequireUserGuildPermissionAttribute(GuildPermission perm) => gPerm = perm;

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (context is CustomCommandContext)
                return Task.FromResult(PreconditionResult.FromSuccess());


            var guildUser = context.User as IGuildUser;
            if (guildUser.GuildPermissions.Has(gPerm))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Você não tem as permissões necessárias para executar esse comando."));
        }
    }
}
