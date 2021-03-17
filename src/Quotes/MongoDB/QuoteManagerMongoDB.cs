using System;
using System.Collections.Generic;
using quoteblok2net.quotes;

namespace quoteblok2net
{
    public class QuoteManagerMongoDB : IQuoteManager
    {
        public bool Add(ulong serverID, ulong userID, ulong messageID, string quote)
        {
            throw new NotImplementedException();
        }

        public bool Add(IQuote quote)
        {
            throw new NotImplementedException();
        }

        public bool Edit(ulong serverID, int index, string quote)
        {
            throw new NotImplementedException();
        }

        public bool Edit(int serverID, int index, string quote)
        {
            throw new NotImplementedException();
        }

        public IQuote Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQuote Get(ulong serverID, int index)
        {
            throw new NotImplementedException();
        }

        public IQuote Get(ulong messageID)
        {
            throw new NotImplementedException();
        }

        public List<IQuote> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<IQuote> GetAll(ulong serverID)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }

        public int GetCount(ulong serverID)
        {
            throw new NotImplementedException();
        }

        public void Import(ulong serverID, ulong userID, ulong msgID)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ulong serverID, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ulong messageID)
        {
            throw new NotImplementedException();
        }
    }
}