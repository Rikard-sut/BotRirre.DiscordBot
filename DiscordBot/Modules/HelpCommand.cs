using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public HelpCommand(CommandService service)
        {
            _commandService = service;
        }

        [Command("Help")]
        public async Task Help()
        {
            List<ModuleInfo> modules = _commandService.Modules.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();


            foreach (ModuleInfo info in modules)
            {
                var commands = info.Commands.ToList();
                string embedGroupText = info.Summary ?? "No description available\n";
                if (info.Group != null)
                    embedBuilder.AddField(info.Group, "\n" + await BuildDescription(commands));
                else
                {
                    foreach (CommandInfo command in info.Commands)
                    {
                        string embedFieldText = command.Summary ?? "No description available";
                        embedBuilder.AddField(command.Name, embedFieldText);
                    }
                }

            }

            await ReplyAsync("Here's a list of commands and their description: \nUse ! as a prefix", false, embedBuilder.Build());
        }

        public Task<string> BuildDescription(List<CommandInfo> info)
        {
            var sb = new StringBuilder();

            foreach(var command in info)
            {
                string embedFieldText = command.Summary ?? "No description available";
                sb.Append(command.Name).Append(' ');
                sb.Append(embedFieldText);
                sb.AppendLine();
            }
            return Task.FromResult(sb.ToString());
        }
    }
}
