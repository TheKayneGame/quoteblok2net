using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenu
{
    class RoleMenu
    {
        
        public long guildID;
        public long userID;
        public long messageID;
        public string text;
        public DateTime date;
        public List<EmojiRoleBinding> emojiRoleBindings = new List<EmojiRoleBinding>();       
    }

    class EmojiRoleBinding
    {
        public Emoji emoji;
        public long roleID;
    }
}
