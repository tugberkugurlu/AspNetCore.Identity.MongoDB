using System;

namespace AspNetCore.Identity.MongoDB.Models
{
    public class ConfirmationOccurrence : Occurrence
    {
        public ConfirmationOccurrence() : base()
        {
        }

        public ConfirmationOccurrence(DateTime confirmedOn) : base(confirmedOn)
        {
        }
    }
}
