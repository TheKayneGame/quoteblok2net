using Discord;
using Discord.Commands;
using quoteblok2net.RoleMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.Commands.Modules
{
    [Group("rolemenu")]
    class RoleMenuModule : ModuleBase<SocketCommandContext>
    {
        RoleMenuManager roleMenuManager = RoleMenuManager.GetInstance();
        [Command("create")]
        [RequireUserPermission(Discord.GuildPermission.ManageRoles)]
        public async Task CreateRoleMenu([Remainder] string text)
        {
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0 || conMsg.MentionedEveryone)
            {
                await ReplyAsync("Mentions are not allowed in text");
                return;
            }
            IUserMessage message = await Context.Channel.SendMessageAsync(text);
            roleMenuManager.Create(Context.Guild.Id,Context.User.Id,message.Id,text);
        }
    }
}
