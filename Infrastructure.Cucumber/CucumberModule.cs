using Ninject.Modules;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITestPublisher>().To<CucumberTestPublisher>();
            Bind<ITestRunner>().To<CucumberTestRunner>();
        }
    }
}