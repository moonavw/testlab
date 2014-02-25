using Ninject;
using TestLab.Domain;
using TestLab.Infrastructure.Cucumber;
using TestLab.Infrastructure.EntityFramework;
using TestLab.Infrastructure.Git;
using TestLab.Infrastructure.Ruby;
using TestLab.Infrastructure.Zip;

namespace TestLab.Application
{
    public class Bootstrapper
    {
        public static void Initialize(IKernel kernel)
        {
            kernel.Load(
                new EntityFrameworkModule(),
                new CucumberModule(),
                new GitModule(),
                new RubyModule(),
                new ZipModule()
                );

            kernel.Bind<ITestService>().To<TestService>();
        }
    }
}