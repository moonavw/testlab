using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestQueueJobHandler : TestJobHandlerBase<TestQueue>
    {
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;

        public TestQueueJobHandler(IUnitOfWork uow, IArchiver archiver, IEnumerable<ITestDriver> drivers)
            : base(uow)
        {
            _archiver = archiver;
            _drivers = drivers;
        }

        protected override async Task Run(TestQueue job)
        {
            var session = job.Session;
            var project = session.Project;
            var build = session.Build;
            var agent = job.Agent;

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            //extract
            Trace.TraceInformation("Extract TestBuild to agent {0}", agent.Name);
            await _archiver.Extract(build.SharedPath, build.LocalPath);

            //get runs NotStarted/NotCompleted
            var pendingRuns = (from e in job.Runs
                               where e.Started == null || e.Completed == null
                               select e).ToList();

            //reset
            pendingRuns.ForEach(z =>
            {
                z.Started = null;
                z.Completed = null;
                z.Result = new TestResult();
            });
            await Uow.CommitAsync();

            //start
            Trace.TraceInformation("Start TestSession {0} with {2} NotStarted/NotCompleted TestRuns on Agent {1}", session, agent, pendingRuns.Count);
            foreach (var run in pendingRuns)
            {
                Trace.TraceInformation("Start TestRun {0} on Agent {1}", run, agent);

                run.Started = DateTime.Now;
                await Uow.CommitAsync();

                run.Result = await driver.Run(run);

                run.Completed = DateTime.Now;
                await Uow.CommitAsync();

                Trace.TraceInformation("Complete TestRun {0} on Agent {1}", run, agent);
            }

            Trace.TraceInformation("Complete TestSession {0} with {2} TestRuns on Agent {1}", session, agent, pendingRuns.Count);
        }
    }
}