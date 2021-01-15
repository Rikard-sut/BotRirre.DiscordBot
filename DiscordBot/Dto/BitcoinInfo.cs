using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace DiscordBot.Dto
{
    public class BitcoinInfo
    {
        [JsonPropertyName("15m")]
        public double Fifteenminutesago { get; set; }

        [JsonPropertyName("last")]
        public double Last { get; set; }

        [JsonPropertyName("buy")]
        public double Buy { get; set; }

        [JsonPropertyName("sell")]
        public double Sell { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Most recent market price of Bitcoin = ").Append(Last).Append(" ").Append(Symbol).AppendLine();

            return sb.ToString();
        }
    }
}
