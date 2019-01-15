using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Pluralsight.MaintainableUnitTests.ModelingClassDependencies
{
    public class TemporalCouplingTests
    {
        //For example, Save must not be called before add
        //But is this not a negative test?
        //It is a real requirement in this case. Save must not be called before add

        [Fact]
        public void GenerateDataOfRepoFactory_AddsNothingAfterSave()
        {
            var repo = new Mock<IRepository<FinancialTarget>>();

            bool saveCalled = false;
            bool addCalledAfterSave = false;

            repo.Setup(r => r.Save()).Callback(() => saveCalled = true);
            repo.Setup(r => r.Add(It.IsAny<FinancialTarget>())).Callback(() => addCalledAfterSave = saveCalled);

            FinancialService service = new FinancialService(repo.Object);

            service.GenerateData(() => repo.Object);

            Assert.False(addCalledAfterSave);
        }

        [Fact]
        public void GenerateDataOfRepoFactory_AddCalled()
        {
            var repo = new Mock<IRepository<FinancialTarget>>();
            FinancialService service = new FinancialService(repo.Object);

            service.GenerateData(() => repo.Object);

            repo.Verify(r => r.Add(It.IsAny<FinancialTarget>()), Times.AtLeastOnce());
        }

        [Fact]
        public void GenerateDataOfRepoFactory_SaveCalled()
        {
            var repo = new Mock<IRepository<FinancialTarget>>();
            FinancialService service = new FinancialService(repo.Object);

            service.GenerateData(() => repo.Object);

            repo.Verify(r => r.Save(), Times.AtLeastOnce());
        }
    }
}
