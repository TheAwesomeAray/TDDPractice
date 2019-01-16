using System.Collections.Generic;
using System.Linq;

namespace Pluralsight.MaintainableUnitTests.DesignByContract
{
    class StringList : List<string>
    {
        public int Count { get; set; }
        public void Append(string value) { }
        public int AverageLength { get { return 0; } }

    }

    class Program
    {
        public void DoSomething<T>(List<T> list)
        {
            list.Append(default(T));
        }

        public void Run()
        {
            DoSomething(new StringList());
        }
    }
}
