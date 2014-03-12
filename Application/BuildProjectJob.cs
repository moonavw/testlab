using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class BuildProjectJob : IJob
    {
        private readonly IUnitOfWork _uow;
        private readonly ITestBuilder _builder;
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<ISourcePuller> _pullers;

        public BuildProjectJob(IUnitOfWork uow,
                               IEnumerable<ISourcePuller> pullers,
                               ITestBuilder builder,
                               IArchiver archiver,
                               IEnumerable<ITestDriver> drivers)
        {
            _uow = uow;
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
            _drivers = drivers;
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

            Trace.TraceInformation("Start Build for {0}", project);

            //pull
            await puller.Pull(project.RepoPathOrUrl, project.WorkDir);
            Trace.TraceInformation("Pulled Source Code for {0} from {1}", project, project.RepoPathOrUrl);

            //build
            project.Build = await _builder.Build(project);
            Trace.TraceInformation("Built Source Code for {0} to output {1}", project, project.BuildOutputPath);

            await _uow.CommitAsync();

            //archive
            await _archiver.Archive(project.BuildOutputDir, project.Build.Location);
            Trace.TraceInformation("Archived Build for {0} to {1}", project, project.Build.Location);

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this project");

            //publish
            var tests = (await driver.Publish(project)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName).ToList();

            toDel.ForEach(z =>
            {
                z.Plans.Clear();
                z.Published = null;
                //project.Cases.Remove(z);
            });
            toAdd.ForEach(z => project.Cases.Add(z));
            Trace.TraceInformation("Published {1} Tests for {0}", project, toAdd.Count);

            await _uow.CommitAsync();
        }
    }
}