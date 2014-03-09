using NPatterns;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;

namespace TestLab.Application
{
    public class RunTestCommandHandler : IHandler<RunTestCommand>
    {
        private readonly IScheduler _scheduler;

        public RunTestCommandHandler(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region IHandler<RunTestCommand> Members

        public void Handle(RunTestCommand message)
        {
            IJobDetail job = JobBuilder.Create<StartTestSessionJob>()
                                       .WithIdentity("job-" + message.TestSessionId + "-" + message.TestCaseId, "TestRuns")
                                       .UsingJobData("TestCaseId", message.TestCaseId)
                                       .UsingJobData("TestSessionId", message.TestSessionId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("trigger-" + message.TestSessionId + "-" + message.TestCaseId, "TestRuns")
                                             .StartNow()
                                             .Build();
            _scheduler.ScheduleJob(job, trigger);
        }

        public Task HandleAsync(RunTestCommand message)
        {
            return Task.Run(() => Handle(message));
        }

        #endregion
    }
}
