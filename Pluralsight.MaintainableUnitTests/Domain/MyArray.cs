using System;

namespace Pluralsight.MaintainableUnitTests
{
    public class MyArray : IMyList
    {
        private int[] Data { get; set; }
        public int Length => Data.Length;

        public int Count => Data.Length;

        public MyArray(int size = 0)
        {
            Data = new int[size];
        }

        public void Append(int value)
        {
            EnlargeBy(1);
            Data[Data.Length - 1] = value;
        }

        public void AppendMany(int[] values)
        {
            int count = Data.Length;
            EnlargeBy(values.Length);
            Array.Copy(values, 0, Data, count, values.Length);
        }

        private void EnlargeBy(int count)
        {
            int[] data = Data;
            Array.Resize(ref data, data.Length + count);
            Data = data;
        }

        public int ElementAt(int index) => Data[index];

        public int GetFirst()
        {
            return Data[0];
        }
    }
}
