using Ninject.Modules;
using TestLab.Domain;

namespace TestLab.Infrastructure.Git
{
    public class GitModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITestPuller>().To<GitTestPuller>();
        }
    }
}