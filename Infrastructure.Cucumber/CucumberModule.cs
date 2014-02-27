using Ninject.Modules;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITestDriver>().To<CucumberTestDriver>();
        }
    }
}