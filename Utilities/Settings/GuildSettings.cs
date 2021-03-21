using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using quoteblok2net.Utilities.Configs;
using System;

namespace quoteblok2net.Utilities.Settings.Guild
{
    public class GuildSettings
    {
        public GuildSettings(long guildID)
        {
            this.guildID = guildID;
            prefixText = ConfigManager.config.defaultPrefix;
        }

        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElement("guildID")]
        public long guildID { get; set; }
        [BsonElement("prefixText")]
        public String prefixText { get; set; }

    }
}
