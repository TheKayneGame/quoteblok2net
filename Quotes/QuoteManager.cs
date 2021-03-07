using System;
using System.Collections.Generic;
using quoteblok2net.quotes;
using SQLite;

namespace quoteblok2net.quotes
{
    public class QuoteManager
    {
        private static QuoteManager _quoteManager;
        public SQLiteConnection _db;

        public QuoteManager()
        {
            _db = new SQLiteConnection("./Databases/Rooivalk.db");
            CreateTableResult result = _db.CreateTable<Quote>();
            Console.WriteLine(result);
        }

        /// <summary>
        /// Retruns instance or Creates one if not exist (Singleton)
        /// </summary>
        /// <returns>QuoteManager instance</returns>
        public static QuoteManager GetInstance() {
            if (_quoteManager == null){
                _quoteManager = new QuoteManager();
            }
            return _quoteManager;
        }

        /*Create*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        /// <param name="messageID"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Add(ulong serverID, ulong userID, ulong messageID, string quote)
        {
            Quote quoteEntry = new Quote();

            quoteEntry.quoteID = Guid.NewGuid();
            quoteEntry.serverID = (long)serverID;
            quoteEntry.userID = (long)userID;
            quoteEntry.msgID = (long)messageID;
            quoteEntry.quote = quote;
            quoteEntry.Date = DateTime.UtcNow;

            int res = _db.Insert(quoteEntry);
            return res > 0;
        }

        /*Read*/

        /// <summary>
        /// Get Quote by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Quote</returns>
        public Quote Get(Guid id)
        {
            return _db.Get<Quote>(t => t.quoteID == id);
        }


        /// <summary>
        /// Get Quote by serverID and index
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <returns>Quote</returns>
        public Quote Get(ulong serverID, int index)
        {
            Guid id = _GetGuid(serverID, index);
            return this.Get(id);
        }

        /// <summary>
        /// Get Quote By messageID
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns>Quote</returns>
        public Quote Get(ulong messageID)
        {
            Guid id = _GetGuid(messageID);
            return this.Get(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Quote> GetAll()
        {
            return _db.Table<Quote>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public List<Quote> GetAll(ulong serverID)
        {
            long id = (long) serverID;
            return _db.Table<Quote>().Where(t => t.serverID == id).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _db.Table<Quote>().Count();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public int GetCount(ulong serverID)
        {
            long id = (long) serverID;
            return _db.Table<Quote>().Count(t => t.serverID == id);
        }

        /*Update*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(Guid id, string quote)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(int serverID, int index, string quote)
        {
            return false;
        }

        /*Remove*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(Guid id)
        {
            return _db.Delete<Quote>(id) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Remove (ulong serverID, int index)
        {
            Guid id = _GetGuid(serverID, index);
            return this.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
         public bool Remove(ulong messageID)
        {
            Guid id = _GetGuid(messageID);
            return this.Remove(id);
        }



        private Guid _GetGuid(ulong serverID, int index)
        {
            return _db.FindWithQuery<Quote>("SELECT * FROM quotes WHERE server_id = ? LIMIT ?,1", (long)serverID, index).quoteID;
        }

        private Guid _GetGuid(ulong messageID)
        {
            return _db.Get<Quote>(t => t.msgID == (long)messageID).quoteID;
        }

    }
}