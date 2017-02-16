using System;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Identity.MongoDB.Models
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "MongoDB serialization needs private setters")]
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

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "MongoDB serialization needs private setters")]
        public override int GetHashCode()
        {
            return Instant.GetHashCode();
        }
    }
}
