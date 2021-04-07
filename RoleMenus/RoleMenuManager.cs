using Discord;
using MongoDB.Driver;
using quoteblok2net.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenus
{
    class RoleMenuManager
    {
        private IMongoCollection<RoleMenu> _db;

        private Dictionary<long, RoleMenu> _roleMenuCache = new Dictionary<long, RoleMenu>();
        private Dictionary<long, DateTime> _timeCache = new Dictionary<long, DateTime>();

        private static RoleMenuManager _instance;

        public static RoleMenuManager GetInstance()
        {
            return _instance ??= new RoleMenuManager();
        }
        public RoleMenuManager()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<RoleMenu>("role_menus");
        }

        public bool Create(ulong guildId, ulong userId, ulong messageId, string text)
        {

            RoleMenu roleMenu = new RoleMenu() { 
                GuildId = (long)guildId, 
                UserId = (long)userId, 
                MessageId = (long)messageId, 
                Text = text, 
                Date = DateTime.UtcNow};
            return Create(roleMenu);
        }

        public bool Create(RoleMenu roleMenu)
        {
            _db.InsertOne(roleMenu);
            return true;
        }

        public RoleMenu AddBinding(ulong messageId, EmoteRoleBinding emoteRoleBinding)
        {
            RoleMenu roleMenu = GetRoleMenu((long)messageId);

            if (!roleMenu.AddRoleBinding(emoteRoleBinding))
                return null;
            UpdateRoleMenu(messageId, roleMenu);
            return roleMenu;

        }

        public void UpdateRoleMenu(ulong messageId, RoleMenu roleMenu)
        {
            DropCacheRoleMenu(messageId);
            _db.ReplaceOne(x => x.MessageId == (long)messageId, roleMenu);

        }
        
        public void UpdateRoleMenuMessage(IUserMessage userMessage, RoleMenu roleMenu)
        {
            
        }

        public bool IsRoleMenu(ulong messageId)
        {
            return _db.Find(x => x.MessageId == (long)messageId).CountDocuments() > 0;
        }

        public RoleMenu GetRoleMenu(long messageId)
        {
            ResetCacheTimer(messageId);
            return _roleMenuCache.ContainsKey(messageId) ? _roleMenuCache[messageId] : CacheRoleMenu(messageId);
        }

        private void ResetCacheTimer(long messageId)
        {
            _timeCache[messageId] = DateTime.UtcNow;
        }



        private RoleMenu CacheRoleMenu(long messageId)
        {
            RoleMenu settings;
            var mongoResult = _db.Find(x => x.MessageId == messageId);

            if (mongoResult.CountDocuments() > 0)
            {
                settings = mongoResult.First();
                lock (_roleMenuCache)
                {
                    _roleMenuCache.Add(messageId, settings);
                    _timeCache[messageId] = DateTime.UtcNow;
                }
                return settings;
            }
            return null;       
        }

        private bool DropCacheRoleMenu(ulong messageId)
        {
            return _roleMenuCache.Remove((long)messageId) && _timeCache.Remove((long)messageId);

        }
    }
}
