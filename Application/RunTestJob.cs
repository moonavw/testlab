using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NPatterns.Messaging;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class RunTestJob : IJob
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IEnumerable<ITestDriver> _drivers;

        public RunTestJob(
            IMessageBus bus,
            IUnitOfWork uow,
            IEnumerable<ITestDriver> drivers)
        {
            _bus = bus;
            _uow = uow;
            _drivers = drivers;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int caseId = dataMap.GetInt("TestCaseId");
            int sessionId = dataMap.GetInt("TestSessionId");

            Trace.TraceInformation("Instance {0} of {1} for test {2} in session {3}", key, GetType().Name, caseId, sessionId);

            RunTest(caseId, sessionId).Wait();
        }

        #endregion

        public async Task RunTest(int caseId, int sessionId)
        {
            var repo = _uow.Repository<TestRun>();
            var run = await repo.FindAsync(caseId, sessionId);
            var session = run.Session;
            var project = session.Project;
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            run.Started = DateTime.Now;
            await _uow.CommitAsync();

            run.Result = await driver.Run(run);
            
            run.Completed = DateTime.Now;
            await _uow.CommitAsync();

            await _bus.PublishAsync(new RunTestCompletedEvent(caseId, sessionId));
        }
    }
}