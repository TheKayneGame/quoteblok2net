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
            if (_instance == null)
            {
                _instance = new RoleMenuManager();
            }
            return _instance;
        }
        public RoleMenuManager()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<RoleMenu>("role_menus");
        }

        public bool Create(ulong guildID, ulong userID, ulong messageID, string text)
        {

            RoleMenu roleMenu = new RoleMenu() { 
                guildID = (long)guildID, 
                userID = (long)userID, 
                messageID = (long)messageID, 
                text = text, 
                date = DateTime.UtcNow};
            return Create(roleMenu);
        }

        public bool Create(RoleMenu roleMenu)
        {
            _db.InsertOne(roleMenu);
            return true;
        }

        public RoleMenu AddBinding(ulong messageID, EmojiRoleBinding emojiRoleBinding)
        {
            RoleMenu roleMenu = GetRoleMenu((long)messageID);
            roleMenu.emojiRoleBindings.Add(emojiRoleBinding);
            UpdateRoleMenu(messageID, roleMenu);
            return roleMenu;

        }

        public void UpdateRoleMenu(ulong messageID, RoleMenu roleMenu)
        {
            DropCacheRoleMenu(messageID);
            _db.ReplaceOne(x => x.messageID == (long)messageID, roleMenu);

        }
        
        public void UpdateRoleMenuMessage(IUserMessage userMessage, RoleMenu roleMenu)
        {
            userMessage.ModifyAsync(msg => msg.Content = roleMenu.getText());
            roleMenu.emojiRoleBindings.ForEach(x =>
            {
                userMessage.AddReactionAsync(x.emote);
            });
        }

        public bool IsRoleMenu(ulong messageID)
        {
            return _db.Find(x => x.messageID == (long)messageID).CountDocuments() > 0;
        }

        public RoleMenu GetRoleMenu(long messageID)
        {
            ResetCacheTimer(messageID);
            return _roleMenuCache.ContainsKey(messageID) ? _roleMenuCache[messageID] : CacheRoleMenu(messageID);
        }

        private void ResetCacheTimer(long messageID)
        {
            _timeCache[messageID] = DateTime.UtcNow;
        }



        private RoleMenu CacheRoleMenu(long messageID)
        {
            RoleMenu settings;
            var mongoResult = _db.Find(x => x.messageID == messageID);

            if (mongoResult.CountDocuments() > 0)
            {
                settings = mongoResult.First();
                lock (_roleMenuCache)
                {
                    _roleMenuCache.Add(messageID, settings);
                    _timeCache[messageID] = DateTime.UtcNow;
                }
                return settings;
            }
            return null;       
        }

        private bool DropCacheRoleMenu(ulong messageID)
        {
            return _roleMenuCache.Remove((long)messageID) && _timeCache.Remove((long)messageID);

        }
    }
}
