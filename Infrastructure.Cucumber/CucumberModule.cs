using Ninject.Modules;
using TestLab.Domain.Services;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberModule:NinjectModule
    {
        public override void Load()
        {
            Bind<ITestBuilder>().To<CucumberTestBuilder>();
            Bind<ITestRunner>().To<CucumberTestRunner>();
            Bind<ITestReporter>().To<CucumberTestReporter>();
        }
    }
}