using MongoDB.Driver;
using quoteblok2net.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quoteblok2net.RoleMenu
{
    class RoleMenuManager
    {
        private IMongoCollection<RoleMenu> _db;

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

            RoleMenu roleMenu = new RoleMenu() { guildID = (long)guildID, userID = (long)userID, messageID = (long)messageID, text =text , date = DateTime.UtcNow};
            return Create(roleMenu);
        }

        public bool Create(RoleMenu roleMenu)
        {
            _db.InsertOne(roleMenu);
            return true;
        }


    }
}
