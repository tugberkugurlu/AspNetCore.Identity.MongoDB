using System;

namespace AspNetCore.Identity.MongoDB.Models
{
    public class FutureOccurrence : Occurrence
    {
        public FutureOccurrence() : base()
        {
        }

        public FutureOccurrence(DateTime willOccurOn) : base(willOccurOn)
        {
        }
    }
}
