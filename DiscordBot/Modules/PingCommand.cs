using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pings the bot.")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }
    }
}
