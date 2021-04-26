using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using Microsoft.Extensions.DependencyInjection;
using quoteblok2net.BotSystem.Configuration;
using quoteblok2net.BotSystem.Settings;
using quoteblok2net.quotes;
using quoteblok2net.RoleMenus;

namespace quoteblok2net
{
    class Program
    {
        private DiscordSocketClient _client;

        private CommandHandler _commandHandler;

        private ReactionHandler _reactionHandler;

        private IServiceProvider _services;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {

        }

        public async Task MainAsync()
        {
            var token = ConfigManager.config.token;
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;

            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(new InteractivityService(_client, TimeSpan.FromSeconds(30), false))
                .AddSingleton<CommandService>()
                .AddSingleton<IQuoteManager>(new QuoteManagerMongoDB())
                .AddSingleton(new GuildSettingsManager())
                .BuildServiceProvider();

            _commandHandler = new CommandHandler(_client, _services);
            _reactionHandler = new ReactionHandler(_client, _services);
            await _commandHandler.Initialise();
            await _reactionHandler.Initialise();

            // Block the program until it is closed.
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            LogAsync(new LogMessage(LogSeverity.Info,"Gateway", $"{_client.CurrentUser} is connected!"));

            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }
    }
}