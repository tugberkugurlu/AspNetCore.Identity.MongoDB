using AspNetCore.Identity.MongoDB.Models;

namespace AspNetCore.Identity.MongoDB.Internal
{
    internal class FutureOccurrenceSerializer : UnixEpochSerializer<FutureOccurrence>
    {
        public FutureOccurrenceSerializer() : base(o => o.Instant, d => new FutureOccurrence(d))
        {
        }
    }
}