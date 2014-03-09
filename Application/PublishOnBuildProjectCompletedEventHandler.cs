using NPatterns;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;

namespace TestLab.Application
{
    public class PublishOnBuildProjectCompletedEventHandler : IHandler<BuildProjectCompletedEvent>
    {
        private readonly IScheduler _scheduler;

        public PublishOnBuildProjectCompletedEventHandler(
             IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region IHandler<BuildProjectCompletedEvent> Members

        public void Handle(BuildProjectCompletedEvent message)
        {
            IJobDetail job = JobBuilder.Create<PublishTestJob>()
                                       .WithIdentity("job-build-publish-" + message.TestProjectId, "TestProjects")
                                       .UsingJobData("TestProjectId", message.TestProjectId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("trigger-build-publish-" + message.TestProjectId, "TestProjects")
                                             .StartNow()
                                             .Build();
            _scheduler.ScheduleJob(job, trigger);
        }

        public Task HandleAsync(BuildProjectCompletedEvent message)
        {
            return Task.Run(() => Handle(message));
        }

        #endregion
    }
}
