using Pluralsight.MaintainableUnitTests;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public abstract class RepositoryTests<T>
{
    public RepositoryTests()
    {
        EmptyStorage();
        ResetIdSequence();
    }

    [Fact]
    public void GetAll_ReturnsNonNull()
    {
        Assert.NotNull(CreateSut().GetAll());
    }

    [Fact]
    public void GetAll_EmptyStorage_ReturnsEmptySequence()
    {
        Assert.False(CreateSut().GetAll().Any());
    }

    [Fact]
    public void GetAll_ReturnsSameReferenceAsFind()
    {
        InitializeStorage(SampleData.Take(1));
        IRepository<T> repo = CreateSut();
        int idToFind = 1;
        T expectedObj = repo.Find(idToFind);
        T actualObj = repo.GetAll().First();

        Assert.Equal(expectedObj, actualObj);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(17)]
    public void GetAll_ObjectPassedToAdd_ResultContainsThatObject(int initialCount)
    {
        IEnumerable<T> data = SampleData.Take(initialCount + 1).ToList();
        InitializeStorage(data.Take(initialCount));
        IRepository<T> repo = CreateSut();
        T expectedObject = data.ElementAt(initialCount);

        repo.Add(expectedObject);
        IEnumerable<T> actualObject = repo.GetAll()
                                          .Where(obj => ReferenceEquals(obj, expectedObject));

        Assert.True(actualObject.Any());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(17)]
    public void GetAll_AddCalledOnce_SequenceContainsOneElementMoreThanBefore(int initialCount)
    {
        IEnumerable<T> data = SampleData.Take(initialCount + 1).ToList();
        InitializeStorage(data.Take(initialCount));
        T newObject = data.ElementAt(initialCount);
        IRepository<T> repo = CreateSut();

        repo.Add(newObject);

        Assert.Equal(initialCount + 1, repo.GetAll().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(17)]
    public void GetAll_AddedOneObjectAndSaved_NewRepoReturnsThatObject(int initialCount)
    {
        IEnumerable<T> data = SampleData.Take(initialCount + 1).ToList();
        InitializeStorage(data.Take(initialCount));
        T obj = data.ElementAt(initialCount);
        IRepository<T> repo = CreateSut();
        repo.Add(obj);
        repo.Save();

        IRepository<T> newRepo = CreateSut();
        IEnumerable<T> actualObj = newRepo.GetAll().Where(x => MemberwiseEqual(x, obj));

        Assert.True(actualObj.Any());
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void GetAll_RemovedOneObjectAndSaved_NewRepoDoesNotReturnThatObject(int storageCount, int indexToRemove)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        int idToRemove = indexToRemove + 1;
        IRepository<T> repo = CreateSut();
        T objToRemove = repo.Find(idToRemove);
        repo.Remove(objToRemove);
        repo.Save();

        IRepository<T> newRepo = CreateSut();
        IEnumerable<T> actualObj = newRepo.GetAll().Where(x => MemberwiseEqual(x, objToRemove));

        Assert.False(actualObj.Any());
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void GetAll_RemovedOneObject_DoesNotReturnThatObject(int storageCount, int indexToRemove)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        int idToRemove = indexToRemove + 1;
        IRepository<T> repo = CreateSut();
        T objToRemove = repo.Find(idToRemove);

        repo.Remove(objToRemove);
        IEnumerable<T> actualObj = repo.GetAll().Where(x => MemberwiseEqual(x, objToRemove));
        
        Assert.False(actualObj.Any());
    }


    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void GetAll_RemovedOneObject_ReturnsSequenceShorterByOneElement(int storageCount, int indexToRemove)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        int idToRemove = indexToRemove + 1;
        IRepository<T> repo = CreateSut();
        T objToRemove = repo.Find(idToRemove);

        repo.Remove(objToRemove);

        Assert.Equal(storageCount - 1, repo.GetAll().Count());
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void Find_ReturnsExpectedObject(int storageCount, int targetIndex)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        T expectedObj = data[targetIndex];
        int id = targetIndex + 1;
        IRepository<T> repo = CreateSut();

        T actualObj = repo.Find(id);

        Assert.True(MemberwiseEqual(expectedObj, actualObj));
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void Find_TwoCallsWithSameIdReturnsSameReference(int storageCount, int targetIndex)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        int id = targetIndex + 1;
        IRepository<T> repo = CreateSut();
        T expectedObj = repo.Find(id);
        T actualObj = repo.Find(id);

        Assert.True(MemberwiseEqual(expectedObj, actualObj));
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void Find_ReturnsSameReferenceAsGetAll(int storageCount, int targetIndex)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        T targetObj = data[targetIndex];
        int id = targetIndex + 1;
        IRepository<T> repo = CreateSut();

        T expectedObj = repo.GetAll().Single(x => MemberwiseEqual(x, targetObj));
        T actualObj = repo.Find(id);

        Assert.Equal(expectedObj, actualObj);
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void Save_ExistingObjectModified_NewRepositoryGetAllReturnsModifiedObject(
        int storageCount, int targetIndex)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);

        T targetObj = data[targetIndex];

        IRepository<T> repo = CreateSut();
        T expectedObj = repo.GetAll().Single(x => MemberwiseEqual(x, targetObj));

        Mutate(expectedObj);
        repo.Save();

        IRepository<T> newRepo = CreateSut();
        IEnumerable<T> actualObj = newRepo.GetAll().Where(x => MemberwiseEqual(x, expectedObj));

        Assert.True(actualObj.Any());
    }

    [Theory]
    [InlineData(17, 0)]
    [InlineData(17, 3)]
    [InlineData(17, 16)]
    public void Save_ExistingObjectModified_NewRepositoryFindReturnsModifiedObject(
    int storageCount, int targetIndex)
    {
        T[] data = SampleData.Take(storageCount).ToArray();
        InitializeStorage(data);
        int id = targetIndex + 1;

        T targetObj = data[targetIndex];

        IRepository<T> repo = CreateSut();
        T expectedObj = repo.GetAll().Single(x => MemberwiseEqual(x, targetObj));

        Mutate(expectedObj);
        repo.Save();

        IRepository<T> newRepo = CreateSut();
        T actualObj = newRepo.Find(id);

        Assert.True(MemberwiseEqual(expectedObj, actualObj));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(17)]
    public void Save_ObjectAdded_IdBecomesPositiveAfterSave(int count)
    {
        IEnumerable<T> data = SampleData.Take(count + 1).ToList();
        InitializeStorage(data.Take(count));
        T newObj = data.ElementAt(count);

        IRepository<T> repo = CreateSut();
        repo.Add(newObj);
        repo.Save();

        Assert.True(GetId(newObj) > 0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(17)]
    public void Remove_AddThenRemove_GetALlDoesNotContainAddedObject(int initialCount)
    {
        IEnumerable<T> data = SampleData.Take(initialCount + 1).ToList();
        InitializeStorage(data.Take(initialCount));
        T newObj = data.ElementAt(initialCount);

        IRepository<T> repo = CreateSut();
        repo.Add(newObj);
        repo.Remove(newObj);

        IEnumerable<T> actualObject = 
            repo.GetAll().Where(obj => ReferenceEquals(obj, newObj));

        Assert.False(actualObject.Any());
    }

    protected abstract bool MemberwiseEqual(T a, T b);
    protected abstract void Mutate(T obj);
    protected abstract int GetId(T newObj);

    protected abstract void InitializeStorage(IEnumerable<T> data);
    protected abstract IEnumerable<T> SampleData { get; }

    protected abstract void ResetIdSequence();
    protected abstract IRepository<T> CreateSut();
    protected abstract void EmptyStorage();
}

