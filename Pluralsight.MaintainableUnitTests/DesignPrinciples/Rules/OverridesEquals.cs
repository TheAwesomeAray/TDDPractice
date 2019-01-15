namespace Pluralsight.MaintainableUnitTests.DesignPrinciples.Rules
{
    public class OverridesEquals<T> : ImplementsMethod<T>
    {
        public OverridesEquals() : base("Equals", typeof(object))
        {
        }
    }
}
