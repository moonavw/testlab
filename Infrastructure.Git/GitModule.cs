using Ninject.Modules;

namespace TestLab.Infrastructure.Git
{
    public class GitModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepoPuller>().To<GitRepoPuller>();
        }
    }
}