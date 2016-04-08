using System;

namespace Dnx.Identity.MongoDB.Models
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
