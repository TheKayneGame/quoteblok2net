using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using Microsoft.Extensions.DependencyInjection;
using quoteblok2net.quotes;

namespace quoteblok2net
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;

        private CommandService _commands;

        private IServiceProvider _services;

        public  CommandHandler(DiscordSocketClient client)
        {
		    _client = client;
        }

        public async Task Initialise() {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new InteractivityService(_client, TimeSpan.FromSeconds(20), false))
                .AddSingleton<CommandService>()
                .BuildServiceProvider();

            _commands = new CommandService();
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            
            _client.MessageReceived += HandleCommandAsync;
        }

        
        private async Task HandleCommandAsync(SocketMessage sockMsg)
        {
            //Console.WriteLine("aaa");
            SocketUserMessage msg = sockMsg as SocketUserMessage;
            if (msg == null) return;

            int argPos = 0;

            if (!(msg.HasCharPrefix('?', ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                msg.Author.IsBot)
                return;

            SocketCommandContext context = new SocketCommandContext(_client, msg);

            IResult result = await _commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: _services);

            if (!result.IsSuccess /*&& result.Error != CommandError.UnknownCommand*/)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }

   
}