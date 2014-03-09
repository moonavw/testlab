﻿using NPatterns;
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
            IJobDetail job = JobBuilder.Create<BuildProjectJob>()
                                       .WithIdentity("job-build" + message.TestProjectId, "TestProjects")
                                       .UsingJobData("TestProjectId", message.TestProjectId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("trigger-build" + message.TestProjectId, "TestProjects")
                                             .StartNow()
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
