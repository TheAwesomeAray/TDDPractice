namespace Pluralsight.MaintainableUnitTests.DesignPrinciples
{
    public class EqualityTests
    {
        public static EqualityTester<T> For<T>(T obj) =>
            new EqualityTester<T>(obj);
    }
}
