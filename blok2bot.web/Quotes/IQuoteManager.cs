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
        /// <param name="guildID"></param>
        /// <param name="userID"></param>
        /// <param name="messageID"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Add(ulong guildID, ulong userID, ulong messageID, string quote);


        public bool Add(IQuote quote);

        /*Read*/

        /// <summary>
        /// Get Quote by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Quote</returns>
        public IQuote Get(Guid id);


        /// <summary>
        /// Get Quote by guildID and index
        /// </summary>
        /// <param name="guildID"></param>
        /// <param name="index"></param>
        /// <returns>Quote</returns>
        public IQuote Get(ulong guildID, int index);

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
        /// <param name="guildID"></param>
        /// <returns></returns>
        public List<IQuote> GetAll(ulong guildID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCount();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public int GetCount(ulong guildID);

        /*Update*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(ulong guildID,int index, string quote);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <param name="index"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(ulong msgID, string quote);

        public bool Edit(Guid quoteID, string quote);

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
        /// <param name="guildID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Remove (ulong guildID, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public bool Remove(ulong messageID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public bool GuildExists(ulong guildID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public bool GuildList(ulong guildID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guildID"></param>
        /// <param name="userID"></param>
        /// <param name="msgID"></param>
        public void Import(ulong guildID, ulong userID, ulong msgID);
    }
}