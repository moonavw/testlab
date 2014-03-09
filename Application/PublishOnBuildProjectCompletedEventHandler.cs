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
            string key = string.Format("publish-build-Project-{0}", message.TestProjectId);
            IJobDetail job = JobBuilder.Create<PublishTestJob>()
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

        public Task HandleAsync(BuildProjectCompletedEvent message)
        {
            return Task.Run(() => Handle(message));
        }

        #endregion
    }
}
