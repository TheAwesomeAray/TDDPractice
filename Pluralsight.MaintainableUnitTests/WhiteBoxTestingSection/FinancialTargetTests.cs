using Moq;
using Pluralsight.MaintainableUnitTests;
using System;
using System.Linq;
using Xunit;


public class FinancialTargetTests
{
    public class StateTests
    {
        [Fact]
        //How do I know that points have been added to the array?
        // - Check Contents - State Testing
        // - Make certain Append Method was called - Interaction Testing (Behavior Verification?)
        public void AddTargetPoints_EmptyArrayAndCount2_ArrayContains3And5()
        {
            var financialTarget = TestableFinancialTarget.Create();
            var array = new MyArray();

            financialTarget.AddTargetPoints(array, 2);

            Assert.Equal(3, array.ElementAt(0));
            Assert.Equal(5, array.ElementAt(1));
        }

        //State Tests rarely come alone
        //We need to make certain we have at least one test that fails without throwing an exception
        //So if there is one element, the test will fail with an exception prior to the below test.
        //If there are more than two elemeents all tests would pass, resulting in a false positive
        [Fact]
        public void AddTargetPoints_EmptyArrayAndCount2_HasLength2()
        {
            var financialTarget = TestableFinancialTarget.Create();
            var array = new MyArray();

            financialTarget.AddTargetPoints(array, 2);

            Assert.Equal(2, array.Length);
        }

        [Fact]
        public void InitializePoints_ReturnsNotNull()
        {
            MyArray array = TestableFinancialTarget.Create().InitializePoints();
            Assert.NotNull(array);
        }

        [Fact]
        public void InitializePoints_ReturnsNonEmptyArray()
        {
            MyArray array = TestableFinancialTarget.Create().InitializePoints();
            Assert.Equal(1, array.Length);
        }

        [Fact]
        public void InitializePoints_ReturnsWithStartValue3()
        {
            MyArray array = TestableFinancialTarget.Create().InitializePoints();
            Assert.Equal(3, array.ElementAt(0));
        }
    }

    public class InteractionTests
    {
        [Fact]
        public void AddTargetPoints_Count2_ArrayAppendCalledTwoTimes()
        {
            var mockArray = new Mock<IMyList>();
            mockArray.Setup(x => x.Append(It.IsAny<int>()));
            TestableFinancialTarget.Create().AddTargetPoints(mockArray.Object, 2);

            mockArray.Verify(x => x.Append(It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public void AddTargetPoints_Count2_ArrayAppendReceives3And5()
        {
            FinancialTarget target = TestableFinancialTarget.Create();
            var mockArray = new Mock<IMyList>();

            int[] expectedValues = { 3, 5 };
            int callIndex = 0;
            bool sequenceCorrect = true;
            mockArray.Setup(x => x.Append(It.IsAny<int>()))
                     .Callback((int x) =>
                     {
                         sequenceCorrect = sequenceCorrect && x == expectedValues[callIndex++];
                     });

            Assert.True(sequenceCorrect);
        }

        [Theory]
        [InlineData(new int[] { })]
        [InlineData(new[] { 1 })]
        public void GetGoldenTarget_ReceivesLessThanTwoPoints_Throws(int[] points)
        {
            var list = new MyArray();
            foreach (var point in points)
                list.Append(point);

            var timeServer = new Mock<ITimeServer>().Object;
            var financialTarget = new FinancialTarget(timeServer);

            Assert.Throws<Exception>(() => financialTarget.GetGoldenTarget(list));
        }

        [Theory]
        [InlineData(new[] { 1, 2, }, 1)]
        //Odd number of elements, ending in even value
        [InlineData(new[] { 2, 4, 1, 7, 8 }, 5)]
        //Even number of elements, ending in odd value
        [InlineData(new[] { 2, 1, 3, 9, 16, 7 }, 4)]
        //[InlineData(new int[] { }, 0)]  //This test case will never pass. The expectation is wrong.
        public void GetGoldenTarget_ReceivesAtLeastTwoTargetPoints_ReturnsExpectedTarget(
            int[] points, int expectedTarget)
        {
            //You don't have to mock everything. Sometimes mocking is more trouble than it is worth
            var list = new MyArray();
            foreach (var point in points)
                list.Append(point);

            var timeServer = new Mock<ITimeServer>().Object;
            var financialTarget = new FinancialTarget(timeServer);

            int targetPoint = financialTarget.GetGoldenTarget(list);

            Assert.Equal(expectedTarget, targetPoint);
        }
    }
}