using System;

namespace Pluralsight.OutsideInTDD.WebAPI.Domain
{
    public class JournalEntry
    {
        public DateTimeOffset Time { get; set; }
        public int Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public int UserId { get; set; }

        public override bool Equals(object obj)
        {
            var cast = obj as JournalEntry;
            return Time == cast.Time
                && Distance == cast.Distance
                && Duration == cast.Duration
                && UserId == cast.UserId;
        }
    }
}