using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Application;

namespace TestLab.AgentService
{
    public partial class Service1 : ServiceBase
    {
        private readonly IKernel kernel;
        private readonly TestAgentService service;

        public Service1()
        {
            InitializeComponent();

            kernel = new StandardKernel();
            Bootstrapper.Initialize(kernel);
            kernel.Rebind<TestLab.Infrastructure.ITestLabUnitOfWork>().To<TestLab.Infrastructure.EF.TestLabUnitOfWork>().InSingletonScope();

            service = kernel.Get<TestAgentService>();
        }

        protected override void OnStart(string[] args)
        {
            service.Start();
        }

        protected override void OnStop()
        {
            service.Stop();
        }
    }
}
