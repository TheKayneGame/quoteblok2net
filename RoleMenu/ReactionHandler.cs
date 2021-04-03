using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenu
{
    class ReactionHandler
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        public ReactionHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task Initialise()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .BuildServiceProvider();

           

            _client.ReactionAdded += ReactionAdded;
         
        }

        public async Task ReactionAdded(Cacheable<IUserMessage, UInt64> message, ISocketMessageChannel channel, SocketReaction reaction)
        {

        }
    }
}
