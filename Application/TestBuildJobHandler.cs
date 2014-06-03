using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestBuildJobHandler : TestJobHandlerBase<TestBuild>
    {
        private readonly IBuilder _builder;
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<IPuller> _pullers;

        public TestBuildJobHandler(IUnitOfWork uow,
                                   IEnumerable<IPuller> pullers,
                                   IBuilder builder,
                                   IArchiver archiver,
                                   IEnumerable<ITestDriver> drivers)
            : base(uow)
        {
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
            _drivers = drivers;
        }

        protected override async Task Run(TestBuild job)
        {
            var build = job;
            var project = build.Project;

            var puller = _pullers.FirstOrDefault(z => z.CanPull(project.RepoPathOrUrl));
            if (puller == null) throw new NotSupportedException("no puller for this project");

            Trace.TraceInformation("Start TestBuild {0}", build);

            //pull
            Trace.TraceInformation("Pull Source Code for {0} from {1}", project, project.RepoPathOrUrl);
            await puller.Pull(project.RepoPathOrUrl, project.SrcDir);

            //build
            Trace.TraceInformation("Build Source Code for {0} to output {1}", project, project.BuildOutputPath);
            await _builder.Build(project.BuildScript, project.SrcDir);

            //archive
            Trace.TraceInformation("Archive Build for {0} to {1}", project, build.LocalPath);
            await _archiver.Archive(project.BuildOutputDir, build.LocalPath);

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this project");

            //publish
            var tests = (await driver.Publish(build)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName).ToList();

            var toUpdate = (from le in project.Cases
                            join re in tests
                            on le.FullName equals re.FullName
                            select new { le, re }).ToList();
            toUpdate.ForEach(z =>
            {
                z.le.Keyword = z.re.Keyword;
            });

            toDel.ForEach(z =>
            {
                z.Plans.Clear();
                project.Cases.Remove(z);
            });
            toAdd.ForEach(z => project.Cases.Add(z));

            await Uow.CommitAsync();

            Trace.TraceInformation("Complete TestBuild {0} with {1} TestCases published", build, toAdd.Count);
        }
    }
}