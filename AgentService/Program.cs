using Ninject;
using System;
using System.ServiceProcess;
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
