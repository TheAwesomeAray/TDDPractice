using System;
using System.Collections.Generic;

namespace Pluralsight.MaintainableUnitTests
{
    public interface IRepository<T> : IDisposable
    {
        // Precondition: IsDisposed == false
        IEnumerable<T> GetAll();
        // Precondition: IsDisposed == false
        void Add(T obj);
        // Precondition: IsDisposed == false
        void Save();
        // Precondition: IsDisposed == false
        void Remove(T obj);
        // Precondition: IsDisposed == false
        T Find(int id); 
    }
}