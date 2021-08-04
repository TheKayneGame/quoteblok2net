using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using quoteblok2net.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenus
{
    public class RoleMenuManager
    {
        private IMongoCollection<RoleMenu> _db;


        private ObjectCache _roleMenuCache = new MemoryCache("role_menus");

        private CacheItemPolicy _policy = new CacheItemPolicy();

        private DiscordSocketClient _client;

        public RoleMenuManager(DiscordSocketClient client, MongoConnector mongoConnector)
        {
            _client = client;

            var options = new CreateIndexOptions
            {
                Name = "expireAfterSecondsIndex",
                ExpireAfter = TimeSpan.MaxValue
            };

            var roleMenuKeysDefinitionBuilder = Builders<RoleMenu>.IndexKeys;
            var indexModel = new CreateIndexModel<RoleMenu>(roleMenuKeysDefinitionBuilder.Ascending(x => x.MessageId));

            _db = mongoConnector.db.GetCollection<RoleMenu>("role_menus");

            _db.Indexes.CreateOne(indexModel);

            _policy.SlidingExpiration = TimeSpan.FromDays(7); //TODO Create Config
        }

        public bool Create(ulong guildId, ulong userId, ulong messageId, ulong channelId,string text)
        {

            RoleMenu roleMenu = new RoleMenu()
            {
                GuildId = (long) guildId,
                UserId = (long) userId,
                MessageId = (long) messageId,
                ChannelId = (long) channelId,
                Text = text,
                Date = DateTime.UtcNow
            };
            return Create(roleMenu);
        }

        public bool Create(RoleMenu roleMenu)
        {
            _db.InsertOne(roleMenu);
            return true;
        }

        public RoleMenu AddBinding(ulong messageId, EmoteRoleBinding emoteRoleBinding)
        {
            RoleMenu roleMenu = GetRoleMenu(messageId);

            if (!roleMenu.AddRoleBinding(emoteRoleBinding))
                return null;
            UpdateRoleMenu(roleMenu);
            return roleMenu;

        }

        public void UpdateRoleMenu(RoleMenu roleMenu)
        {
            
            _db.ReplaceOne(x => x.MessageId == (long) roleMenu.MessageId, roleMenu);
            _roleMenuCache[roleMenu.MessageId.ToString()] = roleMenu;
        }

        public bool UpdateRoleMenuMessage(RoleMenu roleMenu)
        {
            SocketTextChannel channel = (_client.GetChannel((ulong)roleMenu.ChannelId) as SocketTextChannel);
            IUserMessage message = (channel.GetMessageAsync((ulong)roleMenu.MessageId).Result as IUserMessage);
            message.ModifyAsync(x => x.Content = roleMenu.GetText());

            message.AddReactionsAsync(roleMenu.EmoteRoleBindings.Select(x => Emote.Parse(x.Emote)).ToArray());
            return true;
        }

        public bool IsRoleMenu(ulong messageId)
        {
            return _db.Find(x => x.MessageId == (long) messageId).CountDocuments() > 0;
        }

        public List<RoleMenu> GetGuildRoleMenus(ulong guildId)
        {
            return _db.Find(x => x.GuildId == (long)guildId).ToList();
        }

        public RoleMenu GetRoleMenu(ulong messageId)
        {
            RoleMenu roleMenu = _roleMenuCache[messageId.ToString()] as RoleMenu;
            if (roleMenu == null)
            {
                roleMenu = _db.Find(x => x.MessageId == (long) messageId).FirstOrDefault();
                if (roleMenu == null)
                    return null;
                _roleMenuCache.Set(messageId.ToString(), roleMenu, _policy);
            }


            return roleMenu;
        }


    }
}
