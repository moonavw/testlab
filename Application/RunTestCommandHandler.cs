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
            string key = string.Format("run-{1}-TestSession-{0}", message.TestSessionId, message.TestCaseId);
            IJobDetail job = JobBuilder.Create<RunTestJob>()
                                       .WithIdentity(key)
                                       .UsingJobData("TestCaseId", message.TestCaseId)
                                       .UsingJobData("TestSessionId", message.TestSessionId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity(key)
                                             .StartNow()
                                             .ForJob(job)
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
