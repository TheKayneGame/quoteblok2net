using Discord.Commands;
using quoteblok2net.Utilities.Settings.Guild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.Commands.Modules
{
    namespace quoteblok2net.Commands.Modules
    {
        [Group("settings")]
        public class GuildSettingsModule : ModuleBase<SocketCommandContext>
        {
            public GuildSettingsManager guildSettingsManager { get; set; }

            public CommandService CommandService { get; set; }

            [Command("prefix")]
            [Summary("Set the server prefix")]
            [RequireUserPermission(Discord.GuildPermission.ManageGuild)]
            public async Task SetPrefix([Remainder] string prefix)
            {
                long guildID = (long)Context.Guild.Id;
                guildSettingsManager.SetGuildPrefix(guildID, prefix);
                await ReplyAsync($"Set this server's prefix to `{prefix}`");
            }
        }
    }
}
