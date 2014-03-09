using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NPatterns.Messaging;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class StartTestSessionJob : IJob
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IArchiver _archiver;

        public StartTestSessionJob(
            IMessageBus bus,
            IUnitOfWork uow,
            IArchiver archiver)
        {
            _bus = bus;
            _uow = uow;
            _archiver = archiver;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int sessionId = dataMap.GetInt("TestSessionId");

            Trace.TraceInformation("Instance {0} of {1} for test session {2}", key, GetType().Name, sessionId);

            StartSession(sessionId).Wait();
        }

        #endregion

        private async Task StartSession(int sessionId)
        {
            var repo = _uow.Repository<TestSession>();
            var session = await repo.FindAsync(sessionId);

            //start
            session.Started = DateTime.Now;
            await _uow.CommitAsync();

            //extract
            await _archiver.Extract(session.Build.Location, session.BuildDirOnAgent);

            //runs
            var tasks = from t in session.Runs
                        select _bus.PublishAsync(new RunTestCommand(t.TestCaseId, t.TestSessionId));

            await Task.WhenAll(tasks);
        }
    }
}