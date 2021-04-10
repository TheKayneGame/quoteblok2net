using System;
using System.Collections.Generic;
using MongoDB.Driver;
using quoteblok2net.database;

namespace quoteblok2net.BotSystem.Settings
{
    public class GuildSettingsManager
    {
        private static GuildSettingsManager _serverSettingsManager;
        private Dictionary<long, GuildSettings> _settingsCache = new Dictionary<long, GuildSettings>();
        private Dictionary<long, DateTime> _timeCache = new Dictionary<long, DateTime>();
        private IMongoCollection<GuildSettings> _db;

        public GuildSettingsManager()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<GuildSettings>("server_settings");
        }

        public static GuildSettingsManager GetInstance()
        {
            if (_serverSettingsManager == null)
            {
                _serverSettingsManager = new GuildSettingsManager();
            }
            return _serverSettingsManager;
        }

        public String GetGuildPrefix(long guildID)
        {
            String prefix = GetGuildSettings(guildID).prefixText;

            return prefix;
        }

        public bool SetGuildPrefix(long guildID, string prefix)
        {
            GuildSettings settings = GetGuildSettings(guildID);
            settings.prefixText = prefix;
            return SetGuildSettings(guildID, settings);

        }

        private bool SetGuildSettings(long guildID, GuildSettings guildSettings)
        {
            bool result;
            result = _db.ReplaceOne(x => x.guildID == guildID, guildSettings).IsAcknowledged;
            result = DropSettings(guildID);

            return result;
        }

        private GuildSettings GetGuildSettings(long guildID)
        {
            ResetCacheTimer(guildID);
            return _settingsCache.ContainsKey(guildID) ? _settingsCache[guildID] : CacheSettings(guildID);
        }

        private void ResetCacheTimer(long guildID)
        {
            if (!_timeCache.ContainsKey(guildID))
            {
                _timeCache.Add(guildID, DateTime.UtcNow);
            }
            else
            {
                _timeCache[guildID] = DateTime.UtcNow;
            }
        }

        private GuildSettings CacheSettings(long guildID)
        {
            GuildSettings settings;
            var mongoResult = _db.Find(x => x.guildID == guildID);

            if (mongoResult.CountDocuments() > 0)
            {
                settings = mongoResult.First();
            }
            else
            {
                settings = new GuildSettings(guildID);
                _db.InsertOne(settings);
            }

            lock (_settingsCache)
            {
                _settingsCache.Add(guildID, settings);
            }

            return settings;
        }

        private bool DropSettings(long guildID)
        {
            return _settingsCache.Remove(guildID);
        }
    }
}
