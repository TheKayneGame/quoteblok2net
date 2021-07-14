using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenus
{
    class ReactionHandler
    {
        private readonly DiscordSocketClient _client;
        private IServiceProvider _services;
        private RoleMenuManager menuManager;
        public ReactionHandler(DiscordSocketClient client, IServiceProvider service)
        {
            _client = client;
            _services = service;
            menuManager = _services.GetService<RoleMenuManager>();
        }

        public void Initialise()
        {
            
            _client.ReactionAdded += HandleReactionAddedAsync;
            _client.ReactionRemoved += HandleReactionRemovedAsync;
         
        }
        //TODO FIX DUPE CODE
        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, UInt64> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message == null || !message.Author.IsBot)
                return;

            RoleMenu menu = menuManager.GetRoleMenu(message.Id);
            if (menu == null)
                return;

            EmoteRoleBinding emoteRoleBinding = menu.GetBinding(reaction.Emote);
            if (emoteRoleBinding == null)
            {
                await message.RemoveAllReactionsForEmoteAsync(reaction.Emote);
                return;
            }
                
            await (reaction.User.Value as IGuildUser).AddRoleAsync((channel as SocketGuildChannel).Guild.GetRole((ulong)emoteRoleBinding.RoleId));
        }

        public async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, UInt64> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message == null || !message.Author.IsBot)
                return;

            RoleMenu menu = menuManager.GetRoleMenu(message.Id);
            if (menu == null)
                return;
            EmoteRoleBinding emoteRoleBinding = menu.GetBinding(reaction.Emote);
            if (emoteRoleBinding == null)
                return;
            await (reaction.User.Value as IGuildUser).RemoveRoleAsync((channel as SocketGuildChannel).Guild.GetRole((ulong)emoteRoleBinding.RoleId));
        }
    }
}
