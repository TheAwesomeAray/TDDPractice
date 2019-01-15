using System;
using System.Collections.Generic;
using System.Linq;

namespace Pluralsight.MaintainableUnitTests.TestingAgainstInterfaces
{
    internal class MemoryStorage<T> : IStorage<T> 
        where T : class
    {
        private IList<T> Data { get; set; } = new List<T>();
        private int LastId { get; set; }
        private Action<T, int> SetId { get; }

        public MemoryStorage(Action<T, int> setId)
        {
            SetId = setId;
        }

        public IEnumerable<T> GetAll() => Data;

        public void Populate(IEnumerable<T> source)
        {
            Data = new List<T>();
            foreach (T obj in source)
                Add(obj);
            Save();
        }

        public void Clear() => Data.Clear();
        public void ResetId() => LastId = 0;

        public void Add(T obj)
        {
            LastId++;
            SetId(obj, LastId);
            Data.Add(obj);
        }

        public void Save() { }

        public void Remove(T deletedObj)
        {
            int position =
                    Data.Select((obj, index) => new
                        {
                            Index = index,
                            Equal = ReferenceEquals(obj, deletedObj)
                        })
                        .Single(item => item.Equal)
                        .Index;
            Data.RemoveAt(position);
        }
    }
}