using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    [Group("Calc")]
    [Summary("Calculator with basic math functions")]
    public class CalculatorCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Add")]
        [Summary("calculates the sum of integers")]
        public async Task Add([Remainder] string integers)
        {
            var result = integers.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int sum = 0;
            foreach(string number in result)
            {
                sum += int.Parse(number);
            }
            await Context.Channel.SendMessageAsync((sum).ToString());
        }

        [Command("Divide")]
        [Summary("divides two integers")]
        public async Task Divide(int numberOne, int numberTwo)
        {
            await Context.Channel.SendMessageAsync((numberOne + numberTwo).ToString());
        }
    }
}
