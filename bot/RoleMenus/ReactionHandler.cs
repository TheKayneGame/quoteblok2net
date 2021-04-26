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
        public ReactionHandler(DiscordSocketClient client, IServiceProvider service)
        {
            _client = client;
            _services = service;
        }

        public async Task Initialise()
        {
            
            _client.ReactionAdded += HandleReactionAsync;
         
        }

        public async Task HandleReactionAsync(Cacheable<IUserMessage, UInt64> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message == null || !message.Author.IsBot)
                return;
            //TODO On add reaction, check if message is registered by bot. If so, get associated function.
            //If Menu  then get menu and assign role according to Emote

            RoleMenuManager menuManager = _services.GetService<RoleMenuManager>();
            RoleMenu menu = menuManager.GetRoleMenu(message.Id);
            EmoteRoleBinding emoteRoleBinding = menu.GetBinding(reaction.Emote);
            if (emoteRoleBinding != null)
            {
                await (message.Author as IGuildUser).AddRoleAsync((channel as SocketGuildChannel).Guild.GetRole((ulong)emoteRoleBinding.RoleId));
                
            }


        }
    }
}
