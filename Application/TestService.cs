using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestService : ITestService
    {
        private readonly ITestBuilder _builder;
        private readonly ITestArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<ITestPuller> _pullers;

        public TestService(
            IEnumerable<ITestPuller> pullers,
            ITestBuilder builder,
            ITestArchiver archiver,
            IEnumerable<ITestDriver> drivers)
        {
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
            _drivers = drivers;
        }

        public async Task Build(TestBuild build)
        {
            var project = build.Project;
            var puller = _pullers.FirstOrDefault(z => z.CanPull(project));
            if (puller == null) throw new NotSupportedException("no puller for this project");
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this project");

            //pull
            await puller.Pull(project);

            //build
            await _builder.Build(build);

            //archive
            await _archiver.Archive(build);

            //publish
            var tests = (await driver.Publish(build)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName).ToList();
            
            toDel.ForEach(z =>
            {
                z.Plans.Clear();
                z.Results.Clear();
                z.Published = null;
                //project.Cases.Remove(z);
            });
            toAdd.ForEach(z =>
            {
                project.Cases.Add(z);
            });
        }

        public async Task Run(TestSession session)
        {
            var project = session.Plan.Project;
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test session");

            //find last build
            var build = project.Builds.LastOrDefault(z => z.Completed != null && z.Archived != null);
            if (build == null) throw new InvalidOperationException("no archived build for this test session");

            //extract
            await _archiver.Extract(build, session);

            //find tests
            var tests = session.Plan.Cases.Where(z => z.Published != null).ToList();
            if (tests.Count == 0) throw new InvalidOperationException("no tests for this test session");

            //run
            session.Started = DateTime.Now;
            session.Results.Clear();
            foreach (var t in tests)
            {
                var result = await driver.Run(t, build, session);
                session.Results.Add(result);
            }
            session.Completed = DateTime.Now;
        }
    }
}