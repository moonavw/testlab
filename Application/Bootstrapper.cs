using Ninject;
using TestLab.Domain;

namespace TestLab.Application
{
    public class Bootstrapper
    {
        public static void Initialize(IKernel kernel)
        {
            kernel.Load("TestLab.Infrastructure.*.dll");

            kernel.Bind<ITestBuilder>().To<TestBuilder>();
            kernel.Bind<ITestService>().To<TestService>();
        }
    }
}