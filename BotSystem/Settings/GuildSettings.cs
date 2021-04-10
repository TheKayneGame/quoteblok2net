using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using quoteblok2net.BotSystem.Configuration;

namespace quoteblok2net.BotSystem.Settings
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
