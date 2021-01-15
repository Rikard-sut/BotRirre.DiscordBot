using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class NickNameCommand : ModuleBase<SocketCommandContext>
    {
        [Command("nickname")]
        [Summary("Chose your custom nickname")]
        public async Task NickName(string nickname)
        {
            var userToGet = Context.Message.Author;
            var user = await Task.Run(() =>
            {
                 var user = Context.Guild.GetUser(userToGet.Id);
                 return Task.FromResult(user);
            });

            var newNickname = nickname;
            await user.ModifyAsync(x => x.Nickname = newNickname);

        }
    }
}
