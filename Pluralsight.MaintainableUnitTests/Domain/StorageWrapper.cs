using System;
using System.Collections.Generic;
using System.Linq;

namespace Pluralsight.MaintainableUnitTests
{
    public class StorageWrapper<T> : IRepository<T>
        where T : class
    {
        private Func<T, int> GetId { get; }
        private IStorage<T> Storage { get; }
        private IdentityMap<T> IdMap { get; }
        private IList<T> NewObjects { get; } = new List<T>();
        private HashSet<int> RemovedIds { get; } = new HashSet<int>();

        public StorageWrapper(IStorage<T> storage, Func<T, int> getId)
        {
            Storage = storage;
            GetId = getId;

            IdMap = new IdentityMap<T>(getId, Storage.GetAll, LoadById);
        }

        public void Add(T obj) => NewObjects.Add(obj);

        public bool IsDisposed { get; private set; }

        public void Dispose() { IsDisposed = true; }
        
        public void Remove(T obj)
        {
            int id = GetId(obj);
            if (id > 0)
                RemovedIds.Add(id);
            else
                NewObjects.Remove(obj);
        }

        public void Save()
        {
            foreach (T newObj in NewObjects)
                Storage.Add(newObj);

            foreach (T deletedObj in RemovedIds.Select(id => this.Find(id)))
                Storage.Remove(deletedObj);

            Storage.Save();
        }

        public T Find(int id)
        {
            return Storage.GetAll().Concat(NewObjects).Single(x => GetId(x) == id);
        }

        public IEnumerable<T> GetAll()
        {
            return IdMap.GetAll()
                        .Concat(NewObjects)
                        .Where(x => !RemovedIds.Contains(GetId(x)));
        }

        private T LoadById(int id) =>
                Storage.GetAll().SingleOrDefault(obj => GetId(obj) == id);
    }

    public interface IStorage<T> where T : class
    {
        IEnumerable<T> GetAll();
        void Add(T obj);
        void Save();
        void Remove(T deletedObj);
    }
}
