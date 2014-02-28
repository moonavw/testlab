using Ninject.Modules;

namespace TestLab.Infrastructure.Zip
{
    public class ZipModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IArchiver>().To<ZipArchiver>();
        }
    }
}