using Moq;
using Pluralsight.MaintainableUnitTests;
using System;
using Xunit;

public class WhiteBoxTests
{
    [Theory]
    [InlineData(-17)]
    [InlineData(-1)]
    public void Add_TargetPoints_Throws(int count)
    {
        FinancialTarget target = TestableFinancialTarget.Create();
        IMyList list = new Mock<IMyList>().Object;

        Assert.Throws<ArgumentException>(() => target.AddTargetPoints(list, count));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Add_TargetPoints_DoesNotThrow(int count)
    {
        FinancialTarget target = TestableFinancialTarget.Create();
        IMyList list = new Mock<IMyList>().Object;

        target.AddTargetPoints(list, count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    public void AddTargetPoints_AddsSpecifiedNumberOfElements(int count)
    {
        FinancialTarget target = TestableFinancialTarget.Create();
        var list = new Mock<IMyList>();
        list.Setup(x => x.Append(It.IsAny<int>()));
        list.Setup(l => l.AppendMany(It.IsAny<int[]>())).Callback<int[]>(array =>
        {
            foreach (int value in array)
                list.Object.Append(value);
        });

        target.AddTargetPoints(list.Object, count);

        list.Verify(l => l.Append(It.IsAny<int>()), Times.Exactly(count));
    }

    [Theory]
    [InlineData(216, 1, 0, 3)]
    [InlineData(216, 1, 9, 21)]
    [InlineData(216, 1, 71, 145)]
    [InlineData(216, 1, 215, 433)]
    [InlineData(216, 7, 71, 74)]
    [InlineData(216, 6, 71, 74)]
    [InlineData(216, 8, 71, 74)]
    [InlineData(216, 9, 71, 145)]
    [InlineData(216, 11, 71, 145)]
    public void AddTargetPoints_SpecificValueAtPosition(int count, int month, int index, int expectedValue)
    {
        FinancialTarget target = TestableFinancialTarget.Create(month);
        var list = new Mock<IMyList>();
        int addedCount = 0;
        int? valueAtPosition = null;

        list.Setup(l => l.Append(It.IsAny<int>())).Callback<int>(value =>
        {
            if (addedCount++ == index)
                valueAtPosition = value;
        });

        list.Setup(l => l.AppendMany(It.IsAny<int[]>())).Callback<int[]>(array =>
        {
            foreach (int value in array)
                list.Object.Append(value);
        });

        target.AddTargetPoints(list.Object, count);

        Assert.Equal(expectedValue, (int)valueAtPosition);
    }
}

