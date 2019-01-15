using System;
using System.Collections.Generic;

namespace Pluralsight.MaintainableUnitTests
{
    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> GetAll();
        void Add(T obj);
        void Save();
        void Remove(T obj);
        T Find(int id);
    }
}