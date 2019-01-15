using Moq;
using Pluralsight.MaintainableUnitTests;
using Xunit;

public class FinancialServiceTests
{
    [Fact]
    public void GenerateData_CallsDispose()
    {
        FinancialService service = new FinancialService();

        var repo = new Mock<IRepository<FinancialTarget>>();
        service.GenerateData(() => repo.Object);

        repo.Verify(x => x.Dispose(), Times.AtLeastOnce);
    }

    [Fact]
    public void GenerateData_SatisfiesDisposablePattern()
    {
        FinancialService service = new FinancialService();
        var repo = DisposablePattern.For(new Mock<IRepository<FinancialTarget>>().Object);

        service.GenerateData(() => repo.Object);

        repo.VerifyDisposed();
    }
}

