using System;

namespace Pluralsight.OutsideInTDD.WebAPI.Domain
{
    public class JournalEntry
    {
        public DateTime Time { get; set; }
        public decimal Distance { get; set; }
        public TimeSpan Duration { get; set; }
    }
}