﻿using Ninject;
using System;
using System.Configuration.Install;
using System.Reflection;
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
            kernel.Rebind<TestLab.Infrastructure.ITestLabUnitOfWork>().To<TestLab.Infrastructure.EF.TestLabUnitOfWork>().InSingletonScope();
            var agent = kernel.Get<TestAgentService>();

            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;

                    default:
                        agent.Start();

                        do
                        {
                            Console.WriteLine("press Q to quit");
                        }
                        while (Console.ReadKey().Key != ConsoleKey.Q);

                        agent.Stop();

                        break;
                }

            }
            else
            {
                ServiceBase.Run(new TestLabAgentService(agent));
            }
        }
    }
}