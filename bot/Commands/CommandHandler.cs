using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using Microsoft.Extensions.DependencyInjection;
using quoteblok2net.BotSystem.Settings;
using quoteblok2net.quotes;

namespace quoteblok2net
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;

        private CommandService _commands;

        private IServiceProvider _services;

        private GuildSettingsManager _guildSettingsManager;

        public  CommandHandler(DiscordSocketClient client, IServiceProvider service)
        {
		    _client = client;
            _services = service;
        }

        public async Task Initialise() {
            _commands = new CommandService();
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            
            _client.MessageReceived += HandleCommandAsync;
            _guildSettingsManager = (GuildSettingsManager)_services.GetService(typeof(GuildSettingsManager));
        }

        private async Task HandleCommandAsync(SocketMessage sockMsg)
        {
            SocketUserMessage msg = sockMsg as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;

            int argPos = 0;
            long guildID = (long)((SocketGuildChannel)msg.Channel).Guild.Id;
            string prefix = _guildSettingsManager.GetGuildPrefix(guildID);

            if (!(msg.HasStringPrefix(prefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;

            SocketCommandContext context = new SocketCommandContext(_client, msg);

            IResult result = await _commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: _services);

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }

   
}