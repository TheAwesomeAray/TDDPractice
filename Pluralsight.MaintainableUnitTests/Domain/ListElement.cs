namespace Pluralsight.MaintainableUnitTests
{
    internal class ListElement
    {
        public ListElement(int value)
        {
            Value = value;
        }

        public ListElement Next { get; internal set; }
        public int Value { get; internal set; }
    }
}