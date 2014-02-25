using Ninject.Modules;
using TestLab.Domain;

namespace TestLab.Infrastructure.Zip
{
    public class ZipModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITestDeployer>().To<ZipTestDeployer>();
        }
    }
}