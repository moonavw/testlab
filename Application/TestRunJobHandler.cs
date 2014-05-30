using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestRunJobHandler : TestJobHandlerBase<TestRun>
    {
        private readonly IEnumerable<ITestDriver> _drivers;

        public TestRunJobHandler(IUnitOfWork uow, IEnumerable<ITestDriver> drivers)
            : base(uow)
        {
            _drivers = drivers;
        }

        protected override async Task Run(TestRun job)
        {
            var run = job;
            var session = run.Session;
            var project = session.Project;
            var agent = job.Agent;

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            //start
            Debug.WriteLine("Start TestRun {0} on Agent {1}", run, agent);

            run.Result = await driver.Run(run, agent);

            await Uow.CommitAsync();
        }
    }
}
