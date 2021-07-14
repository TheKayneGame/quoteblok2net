using Discord;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace quoteblok2net.RoleMenus
{
    public class RoleMenu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public long GuildId { get; set; }

        public long UserId { get; set; }

        public long MessageId { get; set; }

        public string Text { get; set; }

        public DateTime Date { get;  set; }

        public List<EmoteRoleBinding> EmoteRoleBindings { get; set; }

        public RoleMenu()
        {
            EmoteRoleBindings = new List<EmoteRoleBinding>();
        }


        public string GetText()
        {
            string result = $"{Text}";
            EmoteRoleBindings.ForEach( x =>
            {
                result += $"\n{x.Emote} : `{x.Name}`";
            });
            return result;
        }

        public bool AddRoleBinding(EmoteRoleBinding binding)
        {
            if (EmoteRoleBindings.Exists(x => x.Emote.Equals(binding.Emote.ToString())))
                return false;
            EmoteRoleBindings.Add(binding);
            return true;
        }

        public EmoteRoleBinding GetBinding(IEmote emote)
        {
            return EmoteRoleBindings.FirstOrDefault(x => x.Emote == emote.ToString());
        }
    }

    public class EmoteRoleBinding
    {
        public string Name;

        public string Emote;

        public long RoleId;

        public EmoteRoleBinding(string name, string emote, long roleId)
        {
            this.Name = name;
            this.Emote = emote;
            this.RoleId = roleId;
        }
    }
}
