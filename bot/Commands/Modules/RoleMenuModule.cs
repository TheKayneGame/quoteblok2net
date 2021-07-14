using Discord;
using Discord.Commands;
using Interactivity;
using quoteblok2net.RoleMenus;
using System;
using System.Threading.Tasks;
using quoteblok2net.Utilities;

namespace quoteblok2net.Commands.Modules
{
    [Group("rolemenu")]
    public class RoleMenuModule : ModuleBase<SocketCommandContext>
    {
        public RoleMenuManager RoleMenuManager { get; set; }
        public CommandService CommandService { get; set; }
        public InteractivityService Interactivity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [Command("create")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task CreateRoleMenu([Remainder] string text)
        {
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0 || conMsg.MentionedEveryone)
            {
                await ReplyAsync("Mentions are not allowed in command");
                return;
            }
            IUserMessage message = await Context.Channel.SendMessageAsync(text);
            RoleMenuManager.Create(Context.Guild.Id, Context.User.Id, message.Id, Context.Channel.Id, text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [Command("test", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task Test()
        {
            IUserMessage menuMessage = Context.Message.ReferencedMessage;
            RoleMenuManager.UpdateRoleMenuMessage(RoleMenuManager.GetRoleMenu(menuMessage.Id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [Command("addbinding", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddBinding(IRole role,[Remainder] string text)
        {
            
            IUserMessage menuMessage = Context.Message.ReferencedMessage;
            IMessageChannel channel = Context.Channel;

            if (menuMessage == null || !RoleMenuManager.IsRoleMenu(menuMessage.Id))
            {
                const string noReferenced = "No Menu Referenced";
                Interactivity.DelayedSendMessageAndDeleteAsync(channel, null, TimeSpan.FromSeconds(5), noReferenced);
                return;
            }

            string messageText =
                $"Adding Role `{role.Name}`, React to this message with an Emote.\n" +
                $"This Message will timeout in {Interactivity.DefaultTimeout.TotalSeconds} seconds.";

            IUserMessage message = await channel.SendMessageAsync(messageText);
         
            var result = await Interactivity.NextReactionAsync(x => x.MessageId == message.Id && x.UserId == Context.User.Id);
            
            Interactivity.DelayedDeleteMessageAsync(message, TimeSpan.FromSeconds(5));
            Interactivity.DelayedDeleteMessageAsync(Context.Message, TimeSpan.FromSeconds(5));

            if (result.Value == null)
            {
                const string failedText = "Adding role to menu failed";
                await message.ModifyAsync(msg => msg.Content = failedText);
                return;
            }            

            if (!await menuMessage.TryAddReactionasync(result.Value.Emote))
            {
                String invalidEmoteText =
                    $"`{result.Value.Emote.ToString()}` cannot be used as emote. It's probably an Emote from another server that I'm not in.";
                Interactivity.DelayedSendMessageAndDeleteAsync(Context.Channel, text: invalidEmoteText);
                return;
            }

            string successText = $"Added role `{role.Name}` to the menu with the text `{text}`";
            RoleMenu roleMenu = RoleMenuManager.AddBinding(menuMessage.Id, new EmoteRoleBinding(text, result.Value.Emote.ToString(), (long)role.Id));
            if (roleMenu == null)
            {
                return;
            }
            RoleMenuManager.UpdateRoleMenuMessage(roleMenu);

            await message.ModifyAsync(msg => msg.Content = successText);

        }

        [Command("list", RunMode = RunMode.Async)]
        public async Task ListMenus()
        {
            RoleMenuManager.GetGuildRoleMenus(Context.Guild.Id);
        }

    }
}
