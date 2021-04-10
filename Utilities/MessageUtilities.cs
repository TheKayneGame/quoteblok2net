using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace quoteblok2net.Utilities
{
    public static class MessageUtilities
    {
        public static async Task<bool> TryAddReactionasync(this IUserMessage userMessage, IEmote emote)
        {
            try
            {
                await userMessage.AddReactionAsync(emote);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        } 
    }
}
