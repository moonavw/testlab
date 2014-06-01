using Ninject;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Application;

namespace TestLab.AgentCli
{
    class Program
    {
        static void Main(string[] args)
        {
            //init
            var kernel = new StandardKernel();
            Bootstrapper.Initialize(kernel);
            kernel.Rebind<TestLab.Infrastructure.IUnitOfWork>().To<TestLab.Infrastructure.EF.UnitOfWork>().InSingletonScope();

            //start agent service
            var service = kernel.Get<TestAgentService>();
            service.Initialize(Environment.MachineName);

            var source = new CancellationTokenSource();
            service.Start(source.Token);

            do
            {
                Console.WriteLine("press Q to quit");
            }
            while (Console.ReadKey().Key != ConsoleKey.Q);

            source.Cancel();
        }
    }
}
