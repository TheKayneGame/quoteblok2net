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
        readonly RoleMenuManager _roleMenuManager = RoleMenuManager.GetInstance();
        public CommandService CommandService { get; set; }
        public InteractivityService Interactivity { get; set; }

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
            _roleMenuManager.Create(Context.Guild.Id, Context.User.Id, message.Id, text);
        }

        [Command("test", RunMode = RunMode.Async)]
        public async Task Test(IRole role, [Remainder] string text)
        {
            var result = await Interactivity.NextReactionAsync((x =>
                x.MessageId == Context.Message.Id
                ));
            IUserMessage message = await Context.Channel.SendMessageAsync(text);
            await message.AddReactionAsync(result.Value.Emote);
        }

        [Command("addbinding", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.ManageRoles)]
        public async Task AddBinding(IRole role,[Remainder] string text)
        {
            
            IUserMessage menuMessage = Context.Message.ReferencedMessage;
            IMessageChannel channel = Context.Channel;

            if (menuMessage == null || !_roleMenuManager.IsRoleMenu(menuMessage.Id))
            {
                const string noReferenced = "No Menu Referenced";
                Interactivity.DelayedSendMessageAndDeleteAsync(channel, null, TimeSpan.FromSeconds(5), noReferenced);
                return;
            }

            string messageText =
                $"Adding Role `{role.Name}`, React to this message with an Emote.\n" +
                $"This Message will timeout in {Interactivity.DefaultTimeout.TotalSeconds.ToString()} seconds.";

            IUserMessage message = await channel.SendMessageAsync(messageText);
            
            var result = await Interactivity.NextReactionAsync((x =>
                x.MessageId == message.Id && x.UserId == Context.User.Id
                ));
            
            Interactivity.DelayedDeleteMessageAsync(message, TimeSpan.FromSeconds(5));

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
            RoleMenu roleMenu = _roleMenuManager.AddBinding(menuMessage.Id, new EmoteRoleBinding(text, result.Value.Emote.ToString(), (long)role.Id));
            if (roleMenu == null)
            {
                return;
            }
            await menuMessage.ModifyAsync(msg => msg.Content = roleMenu.GetText());

            await message.ModifyAsync(msg => msg.Content = successText);

        }



    }
}
