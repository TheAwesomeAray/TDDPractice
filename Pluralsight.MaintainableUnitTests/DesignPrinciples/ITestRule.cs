using System.Collections.Generic;

namespace Pluralsight.MaintainableUnitTests.DesignPrinciples
{
    public interface ITestRule
    {
        IEnumerable<string> GetErrorMessages();
    }
}