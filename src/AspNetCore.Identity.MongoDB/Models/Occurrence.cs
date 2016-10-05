using System;

namespace AspNetCore.Identity.MongoDB.Models
{
    public class Occurrence
    {
        public Occurrence() : this(DateTime.UtcNow)
        {
        }

        public Occurrence(DateTime occuranceInstantUtc)
        {
            Instant = occuranceInstantUtc;
        }

        public DateTime Instant { get; private set; }
    }
}
