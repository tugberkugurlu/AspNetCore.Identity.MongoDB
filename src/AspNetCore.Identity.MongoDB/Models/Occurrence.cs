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

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public DateTime Instant { get; private set; }

        protected bool Equals(Occurrence other)
        {
            return Instant.Equals(other.Instant);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((Occurrence) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Instant.GetHashCode();
        }
    }
}
