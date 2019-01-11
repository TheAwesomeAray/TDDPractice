using System;

namespace Pluralsight.OutsideInTDD.WebAPI.Domain
{
    public class JournalEntry
    {
        public DateTime Time { get; set; }
        public int Distance { get; set; }
        public TimeSpan Duration { get; set; }
    }
}