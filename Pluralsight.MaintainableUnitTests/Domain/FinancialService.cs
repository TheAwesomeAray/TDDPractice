using System;

namespace Pluralsight.MaintainableUnitTests
{
    public class FinancialService
    {
        private IRepository<FinancialTarget> TargetsRepository { get; }

        public FinancialService(IRepository<FinancialTarget> targetsRepository)
        {
            TargetsRepository = targetsRepository;
        }

        public FinancialService()
        {
                
        }

        public void GenerateData()
        {
            //use this.TargetsRepository
            //Do not dispose this.TargetsRepository
            //long lived
        }

        public void GenerateData(IRepository<FinancialTarget> repo)
        {
            //repo variable can be garbage collected right after method is called
            
        }

        //Will test the disposable pattern with this method
        public void GenerateData(Func<IRepository<FinancialTarget>> repoFactory)
        {
            //In full control of scope, can be long or short
            using (IRepository<FinancialTarget> repo = repoFactory())
            {
                AddMore(repo);
                repo.Add(new FinancialTarget(new SystemTime()));
                repo.Dispose();
                repo.Save();
                
            }
        }

        private void AddMore(IRepository<FinancialTarget> repo)
        {
            
        }
    }
}
