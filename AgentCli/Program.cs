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

            //start agent service
            var service = kernel.Get<TestAgentService>();
            service.Initialize(Environment.MachineName);

            var source = new CancellationTokenSource();
            service.Start(source.Token);

            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
                Console.WriteLine("press Q to quit");
            }
            source.Cancel();
        }
    }
}
