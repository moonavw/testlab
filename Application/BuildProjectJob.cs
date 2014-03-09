using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NPatterns.Messaging;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class BuildProjectJob : IJob
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly ITestBuilder _builder;
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ISourcePuller> _pullers;

        public BuildProjectJob(
            IMessageBus bus,
            IUnitOfWork uow,
            IEnumerable<ISourcePuller> pullers,
            ITestBuilder builder,
            IArchiver archiver)
        {
            _bus = bus;
            _uow = uow;
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int projectId = dataMap.GetInt("TestProjectId");

            Trace.TraceInformation("Instance {0} of {1} for project {2}", key, GetType().Name, projectId);

            BuildProject(projectId).Wait();
        }

        #endregion

        private async Task BuildProject(int projectId)
        {
            var repo = _uow.Repository<TestProject>();
            var project = await repo.FindAsync(projectId);

            var puller = _pullers.FirstOrDefault(z => z.CanPull(project.RepoPathOrUrl));
            if (puller == null) throw new NotSupportedException("no puller for this project");

            //pull
            await puller.Pull(project.RepoPathOrUrl, project.WorkDir);

            //build
            project.Build = await _builder.Build(project);

            await _uow.CommitAsync();

            //archive
            await _archiver.Archive(project.BuildOutputDir, project.Build.Location);

            await _bus.PublishAsync(new BuildProjectCompletedEvent(projectId));
        }
    }
}