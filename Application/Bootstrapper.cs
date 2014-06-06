using Ninject;
using NPatterns;
using NPatterns.Messaging;
using TestLab.Domain;

namespace TestLab.Application
{
    public class Bootstrapper
    {
        public static void Initialize(IKernel kernel)
        {
            kernel.Load("TestLab.Infrastructure.*.dll");

            kernel.Bind<TestAgentService>().ToSelf().InSingletonScope();

            kernel.Bind<IHandler<TestBuild>>().To<TestBuildJobHandler>();
            kernel.Bind<IHandler<TestQueue>>().To<TestQueueJobHandler>();

            kernel.Bind<IMessageBus>()
                  .ToConstructor(ctorArg => new MessageBus(type => ctorArg.Context.Kernel.GetAll(type)))
                  .InSingletonScope();
        }
    }
}