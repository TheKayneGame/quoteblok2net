using System;
using SQLite;

namespace quoteblok2net.quotes
{
    
[Table("quotes")]
    public class Quote
    {
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
        public string quote
        { get; set; }

        [Column("date")]
        public DateTime Date
        { get; set; }
    }
}