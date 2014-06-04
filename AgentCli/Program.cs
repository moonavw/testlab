using Ninject;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Application;

namespace TestLab.AgentCli
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            Bootstrapper.Initialize(kernel);
            kernel.Rebind<TestLab.Infrastructure.ITestLabUnitOfWork>().To<TestLab.Infrastructure.EF.TestLabUnitOfWork>().InSingletonScope();
            var service = kernel.Get<TestAgentService>();

            if (args.Any(z => z.Equals("-service", StringComparison.OrdinalIgnoreCase)))
            {
                service.Start();

                do
                {
                    Console.WriteLine("press Q to quit");
                }
                while (Console.ReadKey().Key != ConsoleKey.Q);

                service.Stop();
            }
            else
            {
                service.Run();
            }
        }
    }
}
