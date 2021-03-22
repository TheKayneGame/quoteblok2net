using System;
using System.Collections.Generic;
using quoteblok2net.quotes;
using MongoDB.Driver;
using quoteblok2net.database;
using System.Linq;
using quoteblok2net.quotes.SQLite;

namespace quoteblok2net
{
    public class QuoteManagerMongoDB : IQuoteManager
    {
        private IMongoCollection<QuoteMongoDB> _db;
        public QuoteManagerMongoDB()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<QuoteMongoDB>("quotes");

        }
        public bool Add(ulong guildID, ulong userID, ulong messageID, string quote)
        {
            QuoteMongoDB quoteEntry = new QuoteMongoDB() {
                quoteID = Guid.NewGuid(),
                guildID = (long)guildID,
                userID = (long)userID,
                msgID = (long)messageID,
                quoteText = quote,
                date = DateTime.UtcNow
            };

            return Add(quoteEntry);
        }

        public bool Add(IQuote quote)
        {
            QuoteMongoDB quoteMongoDB = quote as QuoteMongoDB ?? new QuoteMongoDB(quote);
            _db.InsertOne(quoteMongoDB);
            return true;
        }

        public bool Edit(ulong guildID, int index, string quote)
        {
            IQuote quoteBuff = Get(guildID, index);
            quoteBuff.quoteText = quote;
            return _db.ReplaceOne(x => x.quoteID == quoteBuff.quoteID, quoteBuff as QuoteMongoDB).IsAcknowledged;
        }

        public bool Edit(ulong msgID)
        {
            throw new NotImplementedException();
        }

        public IQuote Get(Guid id)
        {
            return _db.Find(x => x.quoteID == id).First();
        }

        public IQuote Get(ulong guildID, int index)
        {
            return Get(_GetGuid(guildID, index));
        }

        public IQuote Get(ulong messageID)
        {
            return Get(_GetGuid(messageID));
        }

        public List<IQuote> GetAll()
        {
            return _db.Find(_ => true).ToEnumerable().Cast<IQuote>().ToList();
        }

        public List<IQuote> GetAll(ulong guildID)
        {
            return _db.Find(x => x.guildID == (long)guildID).ToEnumerable().Cast<IQuote>().ToList();
        }

        public int GetCount()
        {
            return (int)_db.EstimatedDocumentCount();
        }

        public int GetCount(ulong guildID)
        {
            return (int)_db.CountDocuments(x => x.guildID == (long)guildID);
        }

        public void Import(ulong guildID, ulong userID, ulong msgID)
        {
            var db = SQLiteConnector.GetInstance();
            db.Table<QuoteSQLite>().ToList().ForEach(x => {
                Add((ulong)x.guildID,(ulong)x.userID,(ulong)x.msgID,x.quoteText);
            });
        }

        public bool Remove(Guid id)
        {
            return _db.DeleteOne(x => x.quoteID == id).IsAcknowledged;
        }

        public bool Remove(ulong guildID, int index)
        {
            return Remove(_GetGuid(guildID,index));
        }

        public bool Remove(ulong messageID)
        {
            return Remove(_GetGuid(messageID));
        }

        private Guid _GetGuid(ulong guildID, int index)
        {
            return _db.Find(x => x.guildID == (long)guildID).Skip(index).Limit(1).First().quoteID;
        }

        private Guid _GetGuid(ulong messageID)
        {
            return _db.Find(x => x.msgID == (long)messageID).Limit(1).First().quoteID;
        }
    }
}