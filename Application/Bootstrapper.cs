using CommonServiceLocator.NinjectAdapter.Unofficial;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using NPatterns;
using NPatterns.Messaging;
using NPatterns.Messaging.IoC;
using TestLab.Domain;

namespace TestLab.Application
{
    public class Bootstrapper
    {
        public static void Initialize(IKernel kernel)
        {
            kernel.Load("TestLab.Infrastructure.*.dll");

            kernel.Bind<ITestBuilder>().To<TestBuilder>();

            kernel.Bind<IHandler<BuildProjectCommand>>().To<BuildProjectCommandHandler>();
            kernel.Bind<IHandler<BuildProjectCompletedEvent>>().To<ArchiveBuildOnBuildProjectCompletedEventHandler>();
            kernel.Bind<IHandler<BuildProjectCompletedEvent>>().To<PublishTestOnBuildProjectCompletedEventHandler>();
            kernel.Bind<IHandler<StartTestSessionCommand>>().To<StartTestSessionCommandHandler>();
            kernel.Bind<IHandler<StartTestRunCommand>>().To<StartTestRunCommandHandler>();

            kernel.Bind<IMessageBus>().To<IocMessageBus>().InSingletonScope();

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));
        }
    }
}