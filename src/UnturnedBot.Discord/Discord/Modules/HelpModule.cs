using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.Discord.Modules
{
    [Name("Ajuda")]
    public class HelpModule : ModuleBase<ICommandContext>
    {
        private CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Esses são os comandos que você pode usar"
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += cmd.Aliases.First() + "\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", embed: builder);
        }

        [Command("help")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync("Não achei nenhum comando parecido com " + Format.Bold(command) + ".");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Comandos parecidos com " + Format.Bold(command) + ": "
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"{Format.Bold("Parâmetros: ")} {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"{Format.Bold("Info: ")} {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", embed: builder.Build());
        }
    }
}
