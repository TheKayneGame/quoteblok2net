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
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0|| conMsg.MentionedEveryone){
                await ReplyAsync("Ga geen mensen lastig vallen");
                return;
            }
            
            string msgBuf = quote;

            await ReplyAsync(msgBuf);

        }

        [Command("help")]
        [Summary("Hulplijst")]
        public async Task Help()
        {     
            EmbedBuilder embedBuilder = new EmbedBuilder();
            List<CommandInfo> commands = new List<CommandInfo>(CommandService.Commands);

            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                string embedNameText = "";
                embedNameText = command.Name;
                if (command.Parameters.Count > 0){
                    embedNameText += ": ";
                
                    foreach (ParameterInfo p in command.Parameters){
                        embedNameText += $" {p.Name}";
                    }   
                }
                

                embedBuilder.AddField(embedNameText, embedFieldText);
            }

            await ReplyAsync("Hier is een Commando lijst: ", false, embedBuilder.Build());
        }

        //Create
        [Command("add")]
        [Summary("Voeg Citaat Toe")]
        public async Task QuoteAdd([Remainder] string quote)
        {  
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0 || conMsg.MentionedEveryone){
                await ReplyAsync("Ga geen mensen lastig vallen");
                return;
            }

            if (conMsg.Content.Length > 500) 
            {
                await ReplyAsync("Ga geen roman Citeren");
            }

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
        [Summary("Roep willekeurige citaat op")]
        public async Task QuoteGet()
        {
            int index = new Random().Next(_quoteManager.GetCount(Context.Guild.Id));
            string quote = index + ". " + _quoteManager.Get(Context.Guild.Id, index).quote;
            await ReplyAsync(quote);
        }

        [Command("get")]
        [Summary("Roep citaat op")]
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
        [Summary("Krijg lijst van citaten")]
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
                if (messageBuff.Length > 1000)
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
        [Command("edit")]
        [Summary("Verander citaat als beheerder (Omzijlt Controle)")]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task QuoteEditAdmin(int index, [Remainder]string quote)
        {
            _quoteManager.Edit(Context.Guild.Id, index,quote);
            await ReplyAsync("Citaat bijgewerkt");
        }

        [Command("edit")]
        [Summary("Verander een van jouw citaten")]
        public async Task QuoteEdit(int index, [Remainder]string quote)
        {
             if (_quoteManager.Get(Context.Guild.Id, index).userID != (long)Context.User.Id)
            {
                await ReplyAsync("Dit is niet jouw Citaat");
                return;
            }
            _quoteManager.Edit(Context.Guild.Id, index,quote);
            await ReplyAsync("Citaat bijgewerkt");
        }


        //Remove

        [Command("remove", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        [Summary("Verwijder citaat als beheerder (Omzijlt Controle)")]
        public async Task QuoteRemoveAdmin(int index)
        {
            await _QuoteRemove(index);
        }

        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Verwijder een van jouw citaten")]
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
                .WithContent(new PageBuilder().WithText("Bevestig"))
                .WithUsers(Context.User)
                .Build();

            var result = await Interactivity.SendConfirmationAsync(request, Context.Channel);

            if (result.Value == true)
            {
                await Context.Channel.SendMessageAsync("Bevestigd :thumbsup:!");
                _quoteManager.Remove(Context.Guild.Id, index);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Afgewezen :thumbsup:!");
            }
        }

        [Command("allcount")]
        [Summary("Hoeveel Totale Citaten")]
        public async Task QuoteAllCount()
        {
            await ReplyAsync(_quoteManager.GetCount().ToString());
        }

        [Command("import")]
        [RequireOwner]
        public async Task QuoteImport()
        {
            _quoteManager.Import(Context.Guild.Id, Context.User.Id, Context.Message.Id);
            await ReplyAsync("Geimporteerd");
        }

    }

}
