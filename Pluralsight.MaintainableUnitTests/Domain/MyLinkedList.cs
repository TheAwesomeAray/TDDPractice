using System;

namespace Pluralsight.MaintainableUnitTests
{
    public class MyLinkedList : IMyList
    {
        private ListElement Head { get; set; }
        private ListElement Tail { get; set; }

        
        public int Count { get; set; }
        

        public int Length => throw new NotImplementedException();

        public void Append(int value)
        {
            ListElement element = new ListElement(value);

            if (Tail == null)
            {
                Head = Tail = element;
            }
            else
            {
                Tail.Next = element;
                Tail = element;
            }

            Count++;
        }

        public void AppendMany(int[] values)
        {
            throw new NotImplementedException();
        }

        public int ElementAt(int index)
        {
            ListElement current = Head;
            for (int i = 0; i < index; i++)
                current = current.Next;

            return current.Value;
        }

        public int GetFirst() => Head.Value;
    }
}
