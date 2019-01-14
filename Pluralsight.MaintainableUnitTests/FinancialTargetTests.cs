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
    }
}