using Discord;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenus
{
    public class RoleMenu
    {
        public ObjectId id;
        public long guildID;
        public long userID;
        public long messageID;
        public string text;
        public DateTime date;
        public List<EmojiRoleBinding> emojiRoleBindings = new List<EmojiRoleBinding>();
        
        public string getText()
        {
            string result = $"{text}";
            emojiRoleBindings.ForEach( x =>
            {
                result += $"\n{x.emote.Name}: `{x.name}`";
            });
            return result;
        }
    }

    public class EmojiRoleBinding
    {
        public string name;
        public IEmote emote;
        public long roleID;

        public EmojiRoleBinding(string name, IEmote emote, long roleID)
        {
            this.name = name;
            this.emote = emote;
            this.roleID = roleID;
        }
    }
}
