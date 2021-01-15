using Discord;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public interface ILoggingService
    {
        Task LogAsync(LogMessage message);
    }
}