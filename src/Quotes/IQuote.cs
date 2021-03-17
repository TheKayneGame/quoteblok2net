using System;

namespace quoteblok2net
{
    public interface IQuote
    {
        Guid quoteID { get; set; }

        long serverID { get; set; }

        long userID { get; set; }

        long msgID { get; set; }

        string quoteText { get; set; }

        DateTime date { get; set; }
    }
}