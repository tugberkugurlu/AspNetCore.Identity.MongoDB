using AspNetCore.Identity.MongoDB.Models;

namespace AspNetCore.Identity.MongoDB.Internal
{
    internal class ConfirmationOccurrenceSerializer : UnixEpochSerializer<ConfirmationOccurrence>
    {
        public ConfirmationOccurrenceSerializer() : base(o => o.Instant, d => new ConfirmationOccurrence(d))
        {
        }
    }
}