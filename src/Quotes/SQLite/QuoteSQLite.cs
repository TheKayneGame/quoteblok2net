using System;
using SQLite;

namespace quoteblok2net.quotes.SQLite
{
    
    [Table("quotes")]
    public class QuoteSQLite : IQuote
    {
        public QuoteSQLite(){

        }
        public QuoteSQLite(IQuote quote) {
            quoteID = quote.quoteID;
            serverID = quote.serverID;
            userID = quote.userID;
            msgID = quote.msgID;
            quoteText = quote.quoteText;
            date = quote.date;
        }

        [PrimaryKey] 
        [Column("id")]
        public Guid quoteID
        { get; set; }

        [Column("server_id")]
        public long serverID
        { get; set; }

        [Column("user_id")]
        public long userID
        { get; set; }

        [Column("message_id")]
        public long msgID
        { get; set; }

        [Column("quote")]
        public string quoteText
        { get; set; }

        [Column("date")]
        public DateTime date
        { get; set; }
    }
}