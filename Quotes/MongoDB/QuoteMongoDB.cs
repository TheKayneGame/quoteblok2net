using System;
using MongoDB.Bson.Serialization.Attributes;

namespace quoteblok2net
{
    public class QuoteMongoDB : IQuote
    {
        public QuoteMongoDB(){

        }
        public QuoteMongoDB(IQuote quote) {
            quoteID = quote.quoteID;
            guildID = quote.guildID;
            userID = quote.userID;
            msgID = quote.msgID;
            quoteText = quote.quoteText;
            date = quote.date;
        }

        [BsonId]
        public Guid quoteID { get; set; }
        [BsonElement("guildID")]
        public long guildID { get; set; }
        [BsonElement("userID")]
        public long userID { get; set; }
        [BsonElement("msgID")]
        public long msgID { get; set; }
        [BsonElement("quoteText")]
        public string quoteText { get; set; }
        [BsonElement("date")]
        public DateTime date { get; set; }
    }
}