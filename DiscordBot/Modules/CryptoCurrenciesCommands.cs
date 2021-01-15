using Discord.Commands;
using DiscordBot.Dto;
using DiscordBot.Services;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("Currency")]
    public class CryptoCurrenciesCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ICryptoCoinService _cryptoService;

        public CryptoCurrenciesCommands(ICryptoCoinService service)
        {
            _cryptoService = service;
        }

        [Command("Bitcoin")]
        public async Task Bitcoin(string currency)
        {
            currency = currency.ToUpper();
            var response = await _cryptoService.GetBitcoinPrice();
            BitcoinInfo bitcoinInfo;
            response.TryGetValue(currency, out bitcoinInfo);

            await ReplyAsync(bitcoinInfo.ToString());
        }
    }
}
