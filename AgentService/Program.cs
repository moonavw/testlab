using Ninject;
using System;
using System.ServiceProcess;
using TestLab.Application;
using TestLab.Infrastructure;

namespace TestLab.AgentService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            Bootstrapper.Initialize(kernel);
            kernel
                .Rebind<ITestLabUnitOfWork>()
                .To<TestLab.Infrastructure.EF.TestLabUnitOfWork>()
                .InSingletonScope();

            kernel
                .Bind<Func<ITestLabUnitOfWork>>()
                .ToMethod(ctx => () => ctx.Kernel.Get<ITestLabUnitOfWork>());

            var agent = kernel.Get<TestAgentService>();

            if (Environment.UserInteractive)
            {
                agent.Start();

                do
                {
                    Console.WriteLine("press Q to quit");
                }
                while (Console.ReadKey().Key != ConsoleKey.Q);

                agent.Stop();
            }
            else
            {
                ServiceBase.Run(new TestLabAgentService(agent));
            }
        }
    }
}
