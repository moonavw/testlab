using Ninject.Modules;

namespace TestLab.Infrastructure.Builder
{
    public class BuilderModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBuilder>().To<ScriptBuilder>();
        }
    }
}