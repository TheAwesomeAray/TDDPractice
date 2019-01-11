using System.Reflection;
using Xunit.Sdk;

namespace Pluralsight.OutsideInTDD
{
    internal class UseDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            new BootStrap().InstallDatabase();
            base.Before(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            new BootStrap().UninstallDatabase();
            base.After(methodUnderTest);
        }
    }
}