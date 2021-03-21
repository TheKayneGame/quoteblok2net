using System;
using System.Collections.Generic;
using MongoDB.Driver;
using quoteblok2net.database;

namespace quoteblok2net.Utilities.Settings.server
{
    public class ServerSettingsManager
    {
        private static ServerSettingsManager _serverSettingsManager;
        private Dictionary<long, ServerSettings> _settingsCache = new Dictionary<long, ServerSettings>();
        private Dictionary<long, DateTime> _timeCache = new Dictionary<long, DateTime>();
        private IMongoCollection<ServerSettings> _db;

        ServerSettingsManager()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<ServerSettings>("server_settings");
        }

        public static ServerSettingsManager GetInstance()
        {
            if (_serverSettingsManager == null)
            {
                _serverSettingsManager = new ServerSettingsManager();
            }
            return _serverSettingsManager;
        }

        public String GetServerPrefix(long serverID)
        {
            String prefix = GetServerSettings(serverID).prefixText;

            return prefix;
        }

        public ServerSettings GetServerSettings(long serverID)
        {
            return _settingsCache.ContainsKey(serverID) ? _settingsCache[serverID] : CacheSettings(serverID);
        }
        private void ResetCacheTimer(long serverID)
        {
            if (!_timeCache.ContainsKey(serverID)){
                _timeCache.Add(serverID,new DateTime());
            }
            else
            {
                _timeCache[serverID] = new DateTime();
            }
        }

        private ServerSettings CacheSettings(long serverID)
        {
            ServerSettings settings = _db.Find(x => x.serverID == serverID).First();
            lock (_settingsCache)
            {
                _settingsCache.Add(serverID, settings);
            }

            return settings;
        }
    }
}
