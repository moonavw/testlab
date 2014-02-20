using Ninject.Modules;
using TestLab.Domain.Services;

namespace TestLab.Infrastructure.Git
{
    public class GitModule:NinjectModule
    {
        public override void Load()
        {
            Bind<ITestSourcePuller>().To<GitTestSourcePuller>();
        }
    }
}