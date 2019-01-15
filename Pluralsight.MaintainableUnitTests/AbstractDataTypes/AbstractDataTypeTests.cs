using Pluralsight.MaintainableUnitTests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

public class AbstractDataTypeTests
{
    // new list => list.Count == 0
    // list => list.Count >= 0 -- Class invariant
    // new list, list.Append(K) => list.GetFirst() == K
    // new list, N x list.Append(K) => list.Count == N

    [Fact]
    public void Count_NewList_ReturnsZero()
    {
        IMyList list = CreateSUT();
        Assert.Equal(0, list.Count);
    }

    [Theory]
    [InlineData(-7)]
    [InlineData(0)]
    [InlineData(15)]
    public void GetFirst_OneValueAppended_ReturnsThatValue(int value)
    {
        IMyList list = CreateSUT();
        list.Append(value);
        Assert.Equal(value, list.GetFirst());
    }

    [Theory]
    [InlineData(new[] { 7 })]
    [InlineData(new[] { 7, 19, 214 })]
    [InlineData(new[] { 2, 1, 6, 9, 8, 40, 106, 11, 720, 4, 15 })]
    public void Count_NValuesAppended_ReturnsN(int[] values)
    {
        IMyList list = CreateSUT();

        //Usually avoid adding loops
        foreach (var value in values)
            list.Append(value);

        Assert.Equal(values.Length, list.Count);
    }

    [Theory]
    [InlineData(new[] { 2, 1, 6, 9, 8, 40, 106, 11, 720, 4, 15 }, 0, 2)]
    [InlineData(new[] { 2, 1, 6, 9, 8, 40, 106, 11, 720, 4, 15 }, 7, 11)]
    [InlineData(new[] { 2, 1, 6, 9, 8, 40, 106, 11, 720, 4, 15 }, 10, 15)]
    public void ElementAt_NValuesAppended_ReturnsValueAtPositionK(int[] values, int index, int expectedValue)
    {
        IMyList list = CreateSUT();

        //Usually avoid adding loops
        foreach (var value in values)
            list.Append(value);

        Assert.Equal(expectedValue, list.ElementAt(index));
    }

    private IMyList CreateSUT()
    {
        return new MyLinkedList();
    }
}

