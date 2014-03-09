using Ninject;
using NPatterns;
using NPatterns.Messaging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
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
            kernel.Bind<IHandler<StartTestSessionCommand>>().To<StartTestSessionCommandHandler>();

            kernel.Bind<IMessageBus>()
                  .ToConstructor(ctorArg => new MessageBus(type => ctorArg.Context.Kernel.GetAll(type)))
                  .InSingletonScope();

            kernel.Bind<IJobFactory>().To<NinjectJobFactory>();

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = kernel.Get<IJobFactory>();
            kernel.Bind<IScheduler>().ToConstant(scheduler).InSingletonScope();

            scheduler.Start();
        }

        public static void Shutdown(IKernel kernel)
        {
            IScheduler scheduler = kernel.Get<IScheduler>();
            scheduler.Shutdown(true);
        }
    }
}