using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TestLab.Application;

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
            kernel.Rebind<TestLab.Infrastructure.ITestLabUnitOfWork>().To<TestLab.Infrastructure.EF.TestLabUnitOfWork>().InSingletonScope();
            var service = kernel.Get<TestAgentService>();

            if (Environment.UserInteractive)
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
                ServiceBase.Run(new TestLabAgentService(service));
            }
        }
    }
}
