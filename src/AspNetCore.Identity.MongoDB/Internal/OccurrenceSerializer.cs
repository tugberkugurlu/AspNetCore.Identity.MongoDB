using AspNetCore.Identity.MongoDB.Models;

namespace AspNetCore.Identity.MongoDB.Internal
{
    internal class OccurrenceSerializer : UnixEpochSerializer<Occurrence>
    {
        public OccurrenceSerializer() : base(o => o.Instant, d => new Occurrence(d))
        {
        }
    }
}