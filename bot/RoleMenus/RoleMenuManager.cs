using Discord;
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
    class RoleMenuManager
    {
        private IMongoCollection<RoleMenu> _db;


        private ObjectCache _roleMenuCache = new MemoryCache("role_menus");

        private CacheItemPolicy _policy = new CacheItemPolicy();

        private static RoleMenuManager _instance;

        public static RoleMenuManager GetInstance()
        {
            return _instance ??= new RoleMenuManager();
        }

        public RoleMenuManager()
        {
            var options = new CreateIndexOptions
            {
                Name = "expireAfterSecondsIndex",
                ExpireAfter = TimeSpan.MaxValue
            };

            var roleMenuKeysDefinitionBuilder = Builders<RoleMenu>.IndexKeys;
            var indexModel = new CreateIndexModel<RoleMenu>(roleMenuKeysDefinitionBuilder.Ascending(x => x.MessageId));

            _db = MongoConnector.GetDatabaseInstance().GetCollection<RoleMenu>("role_menus");

            _db.Indexes.CreateOne(indexModel);

            _policy.SlidingExpiration = TimeSpan.FromDays(7); //TODO Create Config
        }

        public bool Create(ulong guildId, ulong userId, ulong messageId, string text)
        {

            RoleMenu roleMenu = new RoleMenu()
            {
                GuildId = (long) guildId,
                UserId = (long) userId,
                MessageId = (long) messageId,
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
            UpdateRoleMenu(messageId, roleMenu);
            return roleMenu;

        }

        public void UpdateRoleMenu(ulong messageId, RoleMenu roleMenu)
        {
            
            _db.ReplaceOne(x => x.MessageId == (long) messageId, roleMenu);
            _roleMenuCache[messageId.ToString()] = roleMenu;
        }



        public bool IsRoleMenu(ulong messageId)
        {
            return _db.Find(x => x.MessageId == (long) messageId).CountDocuments() > 0;
        }

        public RoleMenu GetRoleMenu(ulong messageId)
        {
            RoleMenu roleMenu = _roleMenuCache[messageId.ToString()] as RoleMenu;
            if (roleMenu == null)
            {
                roleMenu = _db.Find(x => x.MessageId == (long) messageId).First();
                _roleMenuCache.Set(messageId.ToString(), roleMenu, _policy);
            }


            return roleMenu;
        }
    }
}
