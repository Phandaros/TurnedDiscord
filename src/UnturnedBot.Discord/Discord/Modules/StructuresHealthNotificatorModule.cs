using Discord.Commands;
using System.Threading.Tasks;

namespace UnturnedBot.Discord.Discord.Modules
{
    //[Name("Estruturas"), Group("structures")]
    public class StructuresHealthNotificatorModule : ModuleBase<ICommandContext>
    {
        [Command("getdict")]
        public Task GetDictAsync()
        {
            //await ReplyAsync("RocketDirectory: " + await Program.serviceClient.GetRocketDirectoryAsync());
            return Task.CompletedTask;
        }
        [Command("setdict")]
        public Task SetDictAsync(string value)
        {
            return Task.CompletedTask;
            //await Program.serviceClient.SetRocketDirectoryAsync(value);
        }
    }
}
