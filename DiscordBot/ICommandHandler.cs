using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot
{
    public interface ICommandHandler
    {
        Task HandleMessageRecievedAsync(SocketMessage args);
        Task Intialize();
    }
}