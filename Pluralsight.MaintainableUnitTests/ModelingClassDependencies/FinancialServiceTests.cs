using Moq;
using Xunit;

namespace Pluralsight.MaintainableUnitTests.ModelingClassDependencies
{
    public class FinancialServiceTests
    {
        //Do we need these unit tests? Is this too defensive?


        //Skip attribute helps keep track of tests you want to temporarily disable. Say,
        //until after a refactoring
        //This test covers a negative requirement. It proves that someething bad did not happen
        //Why would we fear this?
        //We should instead create a different unit test that covers a positive scenario
        //Do not write negative tests
        //There are too many things that do not happen to cover them
        //Cover true requirements, write positive unit tests, and do not defend against things that should not be
        [Fact(Skip = "Reimplement after x y and z")]
        public void GenerateDataParameterless_DoesNotCallDispose()
        {
            var repo = new Mock<IRepository<FinancialTarget>>();
            FinancialService service = new FinancialService(repo.Object);
            
            service.GenerateData();

            repo.Verify(x => x.Dispose(), Times.Never);
        }

        [Fact]
        public void GenerateData_CallsDispose()
        {
            FinancialService service = new FinancialService(
                new Mock<IRepository<FinancialTarget>>().Object);

            var repo = new Mock<IRepository<FinancialTarget>>();
            service.GenerateData(() => repo.Object);

            repo.Verify(x => x.Dispose());
        }
    }
}
