using System;

namespace Pluralsight.MaintainableUnitTests
{
    public class SystemTime : ITimeServer
    {
        public int GetCurrentMonth()
        {
            return DateTime.Today.Month;
        }
    }
}
