using Ninject.Modules;

namespace TestLab.Infrastructure.EF
{
    public class EfModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>();
        }
    }
}