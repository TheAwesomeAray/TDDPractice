using System;

namespace Pluralsight.MaintainableUnitTests.DesignPrinciples
{
    public sealed class Date : IEquatable<Date>
    {
        public int Year => FullTime.Year;
        public int Month => FullTime.Month;
        public int Day => FullTime.Day;

        private DateTime FullTime { get; }

        public Date(int year, int month, int day)
        {
            FullTime = new DateTime(year, month, day);
        }

        public Date(DateTime d)
            : this(d.Year, d.Month, d.Day)
        { }

        public Date AddDays(int days) =>  new Date(FullTime.AddDays(days));

        public bool Equals(Date other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FullTime.Equals(other.FullTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Date) obj);
        }

        public override int GetHashCode()
        {
            return FullTime.GetHashCode();
        }

        public static bool operator ==(Date a, Date b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        public static bool operator !=(Date a, Date b) => !(a == b);
    }
}
