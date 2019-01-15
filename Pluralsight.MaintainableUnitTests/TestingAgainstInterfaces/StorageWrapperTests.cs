using System;
using System.Collections.Generic;
using System.Linq;

namespace Pluralsight.MaintainableUnitTests.TestingAgainstInterfaces
{
    public class StorageWrapperTests :
        RepositoryTests<StorageWrapperTests.Data>
    {
        public class Data
        {
            public int Id { get; set; }
            public int Content { get; set; }
        }

        protected override IEnumerable<Data> SampleData =>
            Enumerable.Range(1, int.MaxValue).Select(content => new Data() { Content = content });

        private MemoryStorage<Data> FakeStorage { get; } = new MemoryStorage<Data>((obj, id) => obj.Id = id);

        protected override IRepository<Data> CreateSut() => 
            new StorageWrapper<Data>(FakeStorage, x => x.Id);

        protected override void InitializeStorage(IEnumerable<Data> data)
        {
            FakeStorage.Populate(data);
        }

        protected override void EmptyStorage()
        {
            FakeStorage.Clear();
        }

        protected override void ResetIdSequence()
        {
            FakeStorage.ResetId();
        }

        protected override int GetId(Data obj) => obj.Id;

        protected override bool MemberwiseEqual(Data a, Data b) => 
            a.Content == b.Content;

        protected override void Mutate(Data obj) => obj.Content += 1;
    }
}
