using DiscordBot.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public interface ICryptoCoinService
    {
        Task<Dictionary<string, BitcoinInfo>> GetBitcoinPrice();
    }
}