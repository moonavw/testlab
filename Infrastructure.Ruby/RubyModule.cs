using Ninject.Modules;
using TestLab.Domain;

namespace TestLab.Infrastructure.Ruby
{
    public class RubyModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITestBuilder>().To<RubyTestBuilder>();
        }
    }
}