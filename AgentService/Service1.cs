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
        private CancellationTokenSource _source;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //init
            var kernel = new StandardKernel();
            Bootstrapper.Initialize(kernel);
            kernel.Rebind<TestLab.Infrastructure.ITestLabUnitOfWork>().To<TestLab.Infrastructure.EF.TestLabUnitOfWork>().InSingletonScope();

            //start agent service
            var service = kernel.Get<TestAgentService>();
            service.Initialize(Environment.MachineName);

            _source = new CancellationTokenSource();
            service.Start(_source.Token);
        }

        protected override void OnStop()
        {
            _source.Cancel();
        }
    }
}
