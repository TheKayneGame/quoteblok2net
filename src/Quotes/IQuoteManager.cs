using System;
using System.Collections.Generic;

namespace quoteblok2net.quotes
{
    public interface IQuoteManager
    {
        /// <summary>
        /// Retruns instance or Creates one if not exist (Singleton)
        /// </summary>
        /// <returns>QuoteManager instance</returns>

        /*Create*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        /// <param name="messageID"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Add(ulong serverID, ulong userID, ulong messageID, string quote);


        public bool Add(IQuote quote);

        /*Read*/

        /// <summary>
        /// Get Quote by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Quote</returns>
        public IQuote Get(Guid id);


        /// <summary>
        /// Get Quote by serverID and index
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <returns>Quote</returns>
        public IQuote Get(ulong serverID, int index);

        /// <summary>
        /// Get Quote By messageID
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns>Quote</returns>
        public IQuote Get(ulong messageID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<IQuote> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public List<IQuote> GetAll(ulong serverID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCount();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public int GetCount(ulong serverID);

        /*Update*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(ulong serverID,int index, string quote);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(ulong msgID);

        /*Remove*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Remove (ulong serverID, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public bool Remove(ulong messageID);
        public void Import(ulong serverID, ulong userID, ulong msgID);
    }
}