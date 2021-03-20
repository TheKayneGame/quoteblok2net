using System;
using System.Collections.Generic;
using quoteblok2net.quotes;
using MongoDB.Driver;
using quoteblok2net.database;
using System.Linq;

namespace quoteblok2net
{
    public class QuoteManagerMongoDB : IQuoteManager
    {
        private IMongoCollection<QuoteMongoDB> _db;
        public QuoteManagerMongoDB()
        {
            _db = MongoConnector.GetDatabaseInstance().GetCollection<QuoteMongoDB>("quotes");

        }
        public bool Add(ulong serverID, ulong userID, ulong messageID, string quote)
        {
            QuoteMongoDB quoteEntry = new QuoteMongoDB() {
                quoteID = Guid.NewGuid(),
                serverID = (long)serverID,
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

        public bool Edit(ulong serverID, int index, string quote)
        {
            IQuote quoteBuff = Get(serverID, index);
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

        public IQuote Get(ulong serverID, int index)
        {
            return Get(_GetGuid(serverID, index));
        }

        public IQuote Get(ulong messageID)
        {
            return Get(_GetGuid(messageID));
        }

        public List<IQuote> GetAll()
        {
            return _db.Find(_ => true).ToEnumerable().Cast<IQuote>().ToList();
        }

        public List<IQuote> GetAll(ulong serverID)
        {
            return _db.Find(x => x.serverID == (long)serverID).ToEnumerable().Cast<IQuote>().ToList();
        }

        public int GetCount()
        {
            return (int)_db.EstimatedDocumentCount();
        }

        public int GetCount(ulong serverID)
        {
            return (int)_db.CountDocuments(x => x.serverID == (long)serverID);
        }

        public void Import(ulong serverID, ulong userID, ulong msgID)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Guid id)
        {
            return _db.DeleteOne(x => x.quoteID == id).IsAcknowledged;
        }

        public bool Remove(ulong serverID, int index)
        {
            return Remove(_GetGuid(serverID,index));
        }

        public bool Remove(ulong messageID)
        {
            return Remove(_GetGuid(messageID));
        }

        private Guid _GetGuid(ulong serverID, int index)
        {
            return _db.Find(x => x.serverID == (long)serverID).Skip(index).Limit(1).First().quoteID;
        }

        private Guid _GetGuid(ulong messageID)
        {
            return _db.Find(x => x.msgID == (long)messageID).Limit(1).First().quoteID;
        }
    }
}