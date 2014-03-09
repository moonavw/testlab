﻿using NPatterns;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;

namespace TestLab.Application
{
    public class ArchiveOnBuildProjectCompletedEventHandler : IHandler<BuildProjectCompletedEvent>
    {
        private readonly IScheduler _scheduler;

        public ArchiveOnBuildProjectCompletedEventHandler(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region IHandler<BuildProjectCompletedEvent> Members

        public void Handle(BuildProjectCompletedEvent message)
        {
            string key = string.Format("archive-build-Project-{0}", message.TestProjectId);
            IJobDetail job = JobBuilder.Create<ArchiveBuildJob>()
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
