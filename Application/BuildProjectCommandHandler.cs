using NPatterns;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;

namespace TestLab.Application
{
    public class BuildProjectCommandHandler : IHandler<BuildProjectCommand>
    {
        private readonly IScheduler _scheduler;

        public BuildProjectCommandHandler(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region IHandler<BuildProjectCommand> Members

        public void Handle(BuildProjectCommand message)
        {
            string key = string.Format("build-Project-{0}", message.TestProjectId);
            IJobDetail job = JobBuilder.Create<BuildProjectJob>()
                                       .WithIdentity(key)
                                       .UsingJobData("TestProjectId", message.TestProjectId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity(key)
                                             .StartNow()
                                             .ForJob(job)
                                             .Build();
            _scheduler.ScheduleJob(job, trigger);
        }

        public Task HandleAsync(BuildProjectCommand message)
        {
            return Task.Run(() => Handle(message));
        }

        #endregion
    }
}
