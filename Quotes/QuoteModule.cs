using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using quoteblok2net.quotes;
using Interactivity;
using Interactivity.Confirmation;
using Interactivity.Pagination;
using System;
using Discord;

namespace quoteblok2net
{

    [Group("quote")]
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {
        private QuoteManager _quoteManager = QuoteManager.GetInstance();
        public CommandService CommandService { get; set; }
        public InteractivityService Interactivity { get; set; }

        [Command("echo")]
        [Summary("Fuck")]
        public async Task Ping([Remainder] string quote)
        {
            string msgBuf = quote;

            await ReplyAsync(msgBuf);

        }

        [Command("help")]
        public async Task Help()
        {     
            EmbedBuilder embedBuilder = new EmbedBuilder();
            List<CommandInfo> commands = new List<CommandInfo>(CommandService.Commands);

            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                string embedNameText = command.Name + " :";
                
                foreach (ParameterInfo p in command.Parameters){
                    embedNameText += $" {p.Name}";
                }
                             


                embedBuilder.AddField(embedNameText, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }

        //Create
        [Command("add")]
        public async Task QuoteAdd([Remainder] string quote)
        {
            var serverID = Context.Guild.Id;
            var userID = Context.User.Id;
            var messageID = Context.Message.Id;
            _quoteManager.Add(serverID, userID, messageID, quote);

            await ReplyAsync($"Quote `{quote}` is Toegevoegd");
        }

        //Read

        [Command("get")]
        [Alias("")]
        [Priority(-10)]
        public async Task QuoteGet()
        {
            int index = new Random().Next(_quoteManager.GetCount(Context.Guild.Id));
            string quote = index + ". " + _quoteManager.Get(Context.Guild.Id, index).quote;
            await ReplyAsync(quote);
        }

        [Command("get")]
        public async Task QuoteGet(int index)
        {
            if (index > _quoteManager.GetCount(Context.Guild.Id))
            {
                await ReplyAsync("Ongeldig Getal");
                return;
            }
            string quote = index + ". " + _quoteManager.Get(Context.Guild.Id, index).quote;
            await ReplyAsync(quote);
        }

        [Command("list", RunMode = RunMode.Async)]
        public async Task QuoteList()
        {
            List<Quote> quotes = _quoteManager.GetAll(Context.Guild.Id);
            List<PageBuilder> pagedQuoteBuff = new List<PageBuilder>();

            int i = 0;

            if (quotes.Count < 1)
            {
                await ReplyAsync("Geen Citaten Opgeslagen");
                return;
            }
            string messageBuff = "";
            quotes.ForEach(q =>
            {
                string quoteBuff = i++ + ". " + q.quote + '\n';
                messageBuff += quoteBuff;
                if (i % 10 == 0)
                {
                    pagedQuoteBuff.Add(new PageBuilder().WithText(messageBuff));
                    messageBuff = "";
                }

            });
            pagedQuoteBuff.Add(new PageBuilder().WithText(messageBuff));

            var paginator = new StaticPaginatorBuilder()
            .WithUsers(Context.User)
            .WithPages(pagedQuoteBuff)
            .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
            .WithDefaultEmotes()
            .Build();

            await Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));


        }

        //Update

        //Remove


        [Command("remove", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task QuoteRemoveAdmin(int index)
        {
            await _QuoteRemove(index);
        }

        [Command("remove", RunMode = RunMode.Async)]
        public async Task QuoteRemove(int index)
        {
            if (_quoteManager.Get(Context.Guild.Id, index).userID != (long)Context.User.Id)
            {
                await ReplyAsync("Dit is niet jouw Citaat");
                return;
            }
            await _QuoteRemove(index);
        }

        private async Task _QuoteRemove(int index)
        {
            var request = new ConfirmationBuilder()
                .WithContent(new PageBuilder().WithText("Please Confirm"))
                .WithUsers(Context.User)
                .Build();

            var result = await Interactivity.SendConfirmationAsync(request, Context.Channel);

            if (result.Value == true)
            {
                await Context.Channel.SendMessageAsync("Confirmed :thumbsup:!");
                _quoteManager.Remove(Context.Guild.Id, index);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Declined :thumbsup:!");
            }
        }

        [Command("allcount")]
        public async Task QuoteAllCount()
        {
            await ReplyAsync(_quoteManager.GetCount().ToString());
        }

    }

}
