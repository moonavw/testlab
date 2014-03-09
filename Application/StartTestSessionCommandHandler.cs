﻿using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;
using NPatterns;

namespace TestLab.Application
{
    public class StartTestSessionCommandHandler : IHandler<StartTestSessionCommand>
    {
        private readonly IScheduler _scheduler;

        public StartTestSessionCommandHandler(
            IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        #region IHandler<StartTestSessionCommand> Members

        public void Handle(StartTestSessionCommand message)
        {
            IJobDetail job = JobBuilder.Create<StartTestSessionJob>()
                                       .WithIdentity("job-" + message.TestSessionId, "TestSessions")
                                       .UsingJobData("TestSessionId", message.TestSessionId)
                                       .Build();
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity("trigger-" + message.TestSessionId, "TestSessions")
                                             .StartNow()
                                             .Build();
            _scheduler.ScheduleJob(job, trigger);
        }

        public Task HandleAsync(StartTestSessionCommand message)
        {
            return Task.Run(() => Handle(message));
        }

        #endregion
    }
}
