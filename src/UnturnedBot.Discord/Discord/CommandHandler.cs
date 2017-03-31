using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnturnedBot.Discord.Discord.Contexts;
using UnturnedBot.Discord.Discord.Utils;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord.Discord
{
    public class CommandHandler
    {
        public static CommandService commands;

        public CommandHandler(CommandService cmds)
        {
            commands = cmds;
            cmds.AddModulesAsync(Assembly.GetExecutingAssembly());

            DiscordBot.client.MessageReceived += ProcessCommandAsync;
        }

        public async Task ProcessCommandAsync(SocketMessage parameterMessage)
        {
            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;

            CustomCommandContext.dummyMsg = message;

            int argPos = 0;
            if (!(message.HasMentionPrefix(DiscordBot.client.CurrentUser, ref argPos) || message.HasCharPrefix(TokenUtils.PREFIX, ref argPos))) return;

            var context = new CommandContext(DiscordBot.client, message);
            if (DiscordBot.client.CurrentUser.Id == 229740955463450624)
            {
                if (context.Guild.Id == 230100292375543809 || context.User.Id == 229740955463450624)
                {
                    await HandleCommandAsync(context, argPos);
                }
            }
            else
            {
                await HandleCommandAsync(context, argPos);
            }
        }

        public async Task HandleCommandAsync(ICommandContext context, int argPos)
        {
            // Execute the Command, store the result
            var result = await commands.ExecuteAsync(context, argPos, DiscordBot._map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                //System.Logger.Log(result.Error);
                if (result.Error.Equals(CommandError.ParseFailed) || result.Error.Equals(CommandError.BadArgCount) ||
                    result.Error.Equals(CommandError.MultipleMatches))
                {
                    var cmdresult = commands.Search(context, argPos);

                    string cmdstr = null;
                    for (int i = 0; i < cmdresult.Commands.Count; i++)
                    {
                        var cmd = cmdresult.Commands[i];
                        cmdstr += Format.Bold(TokenUtils.PREFIX + cmd.Command.Aliases.First() + " " + string.Join(" ", cmd.Command.Parameters.Select(p => p.Name)));
                        if (i < cmdresult.Commands.Count - 1)
                            cmdstr += " ou ";
                    }
                    EmbedBuilder embed = new EmbedBuilder().WithTitle(":o: **Erro:**").WithColor(Colors.Red).WithDescription(Format.Bold(result.ErrorReason)).AddField(x =>
                    {
                        x.WithName("**Uso correto: **");
                        x.WithValue(cmdstr);
                        x.WithIsInline(true);
                    });
                    await context.Channel.SendMessageAsync("", embed: embed);
                    return;
                }
                await context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":o: **Erro:**").WithColor(Colors.Red).WithDescription(Format.Bold(result.ErrorReason)));
            }
        }
        public async Task<bool> HandleCommandAsync(ICommandContext context, string input)
        {
            // Execute the Command, store the result
            var result = await commands.ExecuteAsync(context, input, DiscordBot._map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                //System.Logger.Log(result.Error);
                var error = result.Error;
                if (error == CommandError.MultipleMatches || error == CommandError.BadArgCount || error == CommandError.ParseFailed)
                {
                    var cmdresult = commands.Search(context, input);
                    if (cmdresult.Commands != null)
                    {
                        string cmdstr = null;
                        for (int i = 0; i < cmdresult.Commands.Count; i++)
                        {
                            var cmd = cmdresult.Commands[i];
                            cmdstr += Format.Bold(TokenUtils.PREFIX + cmd.Command.Aliases.First() + " " + string.Join(" ", cmd.Command.Parameters.Select(p => p.Name)));
                            if (i < cmdresult.Commands.Count - 1)
                                cmdstr += " ou ";
                        }

                        if (context is CommandContext)
                        {
                            EmbedBuilder embed = new EmbedBuilder().WithTitle(":o: **Erro:**").WithColor(Colors.Red).WithDescription(Format.Bold(result.ErrorReason)).AddField(x =>
                            {
                                x.WithName("**Uso correto: **");
                                x.WithValue(cmdstr);
                                x.WithIsInline(true);
                            });
                            await context.Channel.SendMessageAsync("", embed: embed);
                        }
                        else if (context is CustomCommandContext)
                        {
                            Logger.Log("[Erro] Uso correto: " + cmdstr);
                        }
                    }
                }
                else
                {
                    if (context is CommandContext)
                    {
                        await context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":o: **Erro:**").WithColor(Colors.Red).WithDescription(Format.Bold(result.ErrorReason)));
                    }
                    else if (context is CustomCommandContext)
                    {
                        Logger.Log("[Erro] " + result.Error + ": " + result.ErrorReason);
                    }
                }
            }
            else if (context is CustomCommandContext)
            {
                Logger.Log("Comando executado com sucesso!");
            }
            return result.IsSuccess;
        }
    }
}