using Discord.Commands;
using DiscordBot.Services;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Stats : ModuleBase<SocketCommandContext>
    {
        private readonly UserActivityService _service;

        public Stats(UserActivityService service)
        {
            _service = service;
        }

        [Command("stats")]
        public async Task GetStats(string username)
        {
            var userstats = await _service.GetData(username.ToLower());

            if (userstats == null)
            {
                await ReplyAsync("Your username wasnt found in the storage.");
                return;
            }

            var sb = new StringBuilder();

            sb.Append(userstats.Username).AppendLine("'s stats:");

            foreach(var channel in userstats.VoiceChannels)
            {
                sb.Append("Time spent in ").Append(channel.VoiceChannelName)
                    .Append(" = ").Append((int)channel.TimeSpentInChannel).Append(" minutes.").AppendLine();
            }
            await ReplyAsync(sb.ToString());
        }
    }
}
