using System;

namespace quoteblok2net
{
    public class QuoteMongoDB : IQuote
    {
        public Guid quoteID { get; set; }
        public long serverID { get; set; }
        public long userID { get; set; }
        public long msgID { get; set; }
        public string quoteText { get; set; }
        public DateTime date { get; set; }
    }
}