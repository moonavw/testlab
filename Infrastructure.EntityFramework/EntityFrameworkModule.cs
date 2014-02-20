using Ninject.Modules;

namespace TestLab.Infrastructure.EntityFramework
{
    public class EntityFrameworkModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>();
        }
    }
}