using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.Commands;
using quoteblok2net.quotes;
using SQLite;

namespace quoteblok2net.quotes.SQLite
{
    public class QuoteManagerSQLite : IQuoteManager
    {
        private static QuoteManagerSQLite _quoteManager;
        public SQLiteConnection _db;

        public QuoteManagerSQLite()
        {
            _db = new SQLiteConnection("./Databases/Rooivalk.db");
            CreateTableResult result = _db.CreateTable<QuoteSQLite>();
            Console.WriteLine(result);
        }

        /// <summary>
        /// Retruns instance or Creates one if not exist (Singleton)
        /// </summary>
        /// <returns>QuoteManager instance</returns>
        public static QuoteManagerSQLite GetInstance() {
            if (_quoteManager == null){
                _quoteManager = new QuoteManagerSQLite();
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
            QuoteSQLite quoteEntry = new QuoteSQLite() {
                quoteID = Guid.NewGuid(),
                serverID = (long)serverID,
                userID = (long)userID,
                msgID = (long)messageID,
                quoteText = quote,
                date = DateTime.UtcNow
            };

            return Add(quoteEntry);
        }

        public bool Add(IQuote quote){
            QuoteSQLite quoteSQLite = quote as QuoteSQLite ?? new QuoteSQLite(quote);
            int res = _db.Insert(quoteSQLite);
            return res > 0;
        }



        /*Read*/

        /// <summary>
        /// Get Quote by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Quote</returns>
        public IQuote Get(Guid id)
        {
            return _db.Get<QuoteSQLite>(t => t.quoteID == id);
        }


        /// <summary>
        /// Get Quote by serverID and index
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="index"></param>
        /// <returns>Quote</returns>
        public IQuote Get(ulong serverID, int index)
        {
            Guid id = _GetGuid(serverID, index);
            return this.Get(id);
        }

        /// <summary>
        /// Get Quote By messageID
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns>Quote</returns>
        public IQuote Get(ulong messageID)
        {
            Guid id = _GetGuid(messageID);
            return this.Get(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<IQuote> GetAll()
        {
            return _db.Table<QuoteSQLite>().Cast<IQuote>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public List<IQuote> GetAll(ulong serverID)
        {
            long id = (long) serverID;
            return _db.Table<QuoteSQLite>().Where(t => t.serverID == id).Cast<IQuote>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _db.Table<QuoteSQLite>().Count();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public int GetCount(ulong serverID)
        {
            long id = (long) serverID;
            return _db.Table<QuoteSQLite>().Count(t => t.serverID == id);
        }

        /*Update*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool Edit(ulong serverID,int index, string quote)
        {
            IQuote quoteBuff = Get(serverID, index);
            quoteBuff.quoteText = quote;
            return _db.Update(quoteBuff) > 0;
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
            return _db.Delete<QuoteSQLite>(id) > 0;
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
        public void Import(ulong serverID, ulong userID, ulong msgID)
        {
            try {
                System.IO.StreamReader file = new System.IO.StreamReader($"{serverID}");
                string line = "";
                while((line = file.ReadLine()) != null)  
                {  
                    this.Add(serverID, userID, msgID, line);
                }
            } catch (Exception e){
                Console.WriteLine(e);
            }
            
            
        }

        private Guid _GetGuid(ulong serverID, int index)
        {
            return _db.FindWithQuery<QuoteSQLite>("SELECT * FROM quotes WHERE server_id = ? LIMIT ?,1", (long)serverID, index).quoteID;
        }

        private Guid _GetGuid(ulong messageID)
        {
            return _db.Get<QuoteSQLite>(t => t.msgID == (long)messageID).quoteID;
        }

    }
}