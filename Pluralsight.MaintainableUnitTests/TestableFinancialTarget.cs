using System;
using Moq;

namespace Pluralsight.MaintainableUnitTests
{
    public static class TestableFinancialTarget
    {
        public static FinancialTarget Create()
        {
            return new FinancialTarget(new Mock<ITimeServer>().Object);
        }

        internal static FinancialTarget Create(int month)
        {
            var mockTimeServer = new Mock<ITimeServer>();
            mockTimeServer.Setup(x => x.GetCurrentMonth()).Returns(month);
            return new FinancialTarget(mockTimeServer.Object);
        }
    }
}
