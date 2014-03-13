using NPatterns;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class StartTestSessionCommandHandler : IHandler<StartTestSessionCommand>
    {
        private readonly IScheduler _scheduler;
        private readonly IUnitOfWork _uow;

        public StartTestSessionCommandHandler(
            IUnitOfWork uow,
            IScheduler scheduler)
        {
            _uow = uow;
            _scheduler = scheduler;
        }

        #region IHandler<StartTestSessionCommand> Members

        public void Handle(StartTestSessionCommand message)
        {
            HandleAsync(message).Wait();
        }

        public async Task HandleAsync(StartTestSessionCommand message)
        {
            var repo = _uow.Repository<TestSession>();
            var session = await repo.FindAsync(message.TestSessionId);
            //reset session
            session.Started = DateTime.Now;
            session.Completed = null;
            //reset runs
            foreach (var run in session.Runs)
            {
                run.Started = null;
                run.Completed = null;
                run.Result = new TestResult();
            }
            await _uow.CommitAsync();

            var agents = session.GetAgents().ToList();
            for (int i = 0; i < agents.Count; i++)
            {
                string key = string.Format("start-TestSession-{0}-Agent-{1}", message.TestSessionId, agents[i]);
                IJobDetail job = JobBuilder.Create<StartTestSessionJob>()
                                           .WithIdentity(key)
                                           .UsingJobData("TestSessionId", message.TestSessionId)
                                           .UsingJobData("TestAgentIndex", i)
                                           .Build();
                ITrigger trigger = TriggerBuilder.Create()
                                                 .WithIdentity(key)
                                                 .StartNow()
                                                 .ForJob(job)
                                                 .Build();
                _scheduler.ScheduleJob(job, trigger);
            }
        }

        #endregion
    }
}
