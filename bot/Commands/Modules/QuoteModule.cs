using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using quoteblok2net.quotes;
using Interactivity;
using Interactivity.Confirmation;
using Interactivity.Pagination;
using System;
using Discord;

namespace quoteblok2net.modules
{

    [Group("quote")]
    public class QuoteModule : ModuleBase<SocketCommandContext>
    {
        public IQuoteManager quoteManager { get; set; }
        public CommandService CommandService { get; set; }
        public InteractivityService Interactivity { get; set; }

        [Command("echo")]
        [Summary("Echoes Message")]
        public async Task Ping([Remainder] string quote)
        {
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0|| conMsg.MentionedEveryone){
                await ReplyAsync("Mentions are not allowed in quotes.");
                return;
            }
            
            string msgBuf = $"`{quote}`";

            await ReplyAsync(msgBuf);

        }

        [Command("help")]
        [Summary("Lists commands")]
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

            await ReplyAsync("Command List: ", false, embedBuilder.Build());
        }

        //Create
        [Command("add")]
        [Summary("Adds quote")]
        public async Task QuoteAdd([Remainder] string quote)
        {  
            var conMsg = Context.Message;
            if (conMsg.MentionedChannels.Count + conMsg.MentionedRoles.Count + conMsg.MentionedUsers.Count > 0 || conMsg.MentionedEveryone){
                await ReplyAsync("Mentions are not allowed in quotes.");
                return;
            }

            if (conMsg.Content.Length > 500) 
            {
                await ReplyAsync("Quote can't be longer than 500 characters.");
                return;
            }

            var guildID = Context.Guild.Id;
            var userID = Context.User.Id;
            var messageID = Context.Message.Id;
            
            quoteManager.Add(guildID, userID, messageID, quote);

            await ReplyAsync($"Quote `{quote}` has been added.");
        }

        //Read

        [Command("get")]
        [Alias("")]
        [Priority(-10)]
        [Summary("Get random quote")]
        public async Task QuoteGet()
        {
            int index = new Random().Next(quoteManager.GetCount(Context.Guild.Id));
            string quote = index + ". " + quoteManager.Get(Context.Guild.Id, index).quoteText;
            await ReplyAsync(quote);
        }

        [Command("get")]
        [Summary("Get specific quote")]
        public async Task QuoteGet(int index)
        {
            if (index > quoteManager.GetCount(Context.Guild.Id))
            {
                await ReplyAsync("Invalid number.");
                return;
            }
            string quote = index + ". " + quoteManager.Get(Context.Guild.Id, index).quoteText;
            await ReplyAsync(quote);
        }

        [Command("list", RunMode = RunMode.Async)]
        [Summary("List quotes")]
        public async Task QuoteList()
        {
            List<IQuote> quotes = quoteManager.GetAll(Context.Guild.Id);
            List<PageBuilder> pagedQuoteBuff = new List<PageBuilder>();

            int i = 0;

            if (quotes.Count < 1)
            {
                await ReplyAsync("No saved quotes.");
                return;
            }
            string messageBuff = "";
            quotes.ForEach(q =>
            {
                string quoteBuff = i++ + ". " + q.quoteText + '\n';
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
        [Summary("Edit quote as admin (Bypasses check)")]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        public async Task QuoteEditAdmin(int index, [Remainder]string quote)
        {
            quoteManager.Edit(Context.Guild.Id, index,quote);
            await ReplyAsync("Quote Edited.");
        }

        [Command("edit")]
        [Summary("Change quote")]
        public async Task QuoteEdit(int index, [Remainder]string quote)
        {
             if (quoteManager.Get(Context.Guild.Id, index).userID != (long)Context.User.Id)
            {
                await ReplyAsync("This is not your quote.");
                return;
            }
            quoteManager.Edit(Context.Guild.Id, index,quote);
            await ReplyAsync("Quote Edited.");
        }


        //Remove

        [Command("remove", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        [Summary("Delete quote as admin (Bypasses check)")]
        public async Task QuoteRemoveAdmin(int index)
        {
            await _QuoteRemove(index);
        }

        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Delete quote")]
        public async Task QuoteRemove(int index)
        {
            if (quoteManager.Get(Context.Guild.Id, index).userID != (long)Context.User.Id)
            {
                await ReplyAsync("This is not your quote.");
                return;
            }
            await _QuoteRemove(index);
            await ReplyAsync("Quote deleted.");
        }

        private async Task _QuoteRemove(int index)
        {
            var request = new ConfirmationBuilder()
                .WithContent(new PageBuilder().WithText("Confirm"))
                .WithUsers(Context.User)
                .Build();

            var result = await Interactivity.SendConfirmationAsync(request, Context.Channel);

            if (result.Value == true)
            {
                await Context.Channel.SendMessageAsync("Confirmed :thumbsup:!");
                quoteManager.Remove(Context.Guild.Id, index);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Canceled :thumbsup:!");
            }
        }

        [Command("allcount")]
        [RequireOwner]
        public async Task QuoteAllCount()
        {
            await ReplyAsync(quoteManager.GetCount().ToString());
        }

        [Command("import")]
        [RequireOwner]
        public async Task QuoteImport()
        {
            quoteManager.Import(Context.Guild.Id, Context.User.Id, Context.Message.Id);
            await ReplyAsync("Geimporteerd");
        }

    }

}
