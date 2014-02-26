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
        private readonly IEnumerable<ITestPublisher> _publishers;
        private readonly IEnumerable<ITestPuller> _pullers;
        private readonly IEnumerable<ITestRunner> _runners;
        private readonly IUnitOfWork _uow;

        public TestService(
            IEnumerable<ITestPuller> pullers,
            ITestBuilder builder,
            ITestArchiver archiver,
            IEnumerable<ITestPublisher> publishers,
            IEnumerable<ITestRunner> runners,
            IUnitOfWork uow)
        {
            _pullers = pullers;
            _builder = builder;
            _archiver = archiver;
            _publishers = publishers;
            _runners = runners;
            _uow = uow;
        }

        public async Task Build(TestBuild build)
        {
            if (build.Project == null)
                build.Project = await _uow.Repository<TestProject>().FindAsync(build.TestProjectId);
            var project = build.Project;
            var puller = _pullers.FirstOrDefault(z => z.CanPull(project));
            if (puller == null) throw new NotSupportedException("no puller for this project");
            var publisher = _publishers.FirstOrDefault(z => z.Type == project.Type);
            if (publisher == null) throw new NotSupportedException("no publisher for this project");

            //pull
            await puller.Pull(project);
            await _uow.CommitAsync();

            //build
            await _builder.Build(build);
            await _uow.CommitAsync();

            //archive
            await _archiver.Archive(build);
            await _uow.CommitAsync();

            //publish
            var tests = (await publisher.Publish(build)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName + "#" + z.Name).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName + "#" + z.Name).ToList();

            toDel.ForEach(z => project.Cases.Remove(z));
            toAdd.ForEach(z => project.Cases.Add(z));
            await _uow.CommitAsync();
        }

        public async Task Run(TestSession session)
        {
            if (session.Plan == null)
                session.Plan = await _uow.Repository<TestPlan>().FindAsync(session.TestPlanId);
            var project = session.Plan.Project;
            var runner = _runners.FirstOrDefault(z => z.Type == project.Type);
            if (runner == null) throw new NotSupportedException("no runner for this test session");

            //find last build
            var build = project.Builds.LastOrDefault(z => z.Completed != null && z.Archived != null);
            if (build == null) throw new InvalidOperationException("no archived build for this test session");

            //extract
            await _archiver.Extract(build, session.Config);

            //find tests
            var tests = session.Plan.Cases.Where(z => z.Published != null).ToList();
            if (tests.Count == 0) throw new InvalidOperationException("no tests for this test session");

            //run
            session.Results.Clear();
            foreach (var t in tests)
            {
                var result = await runner.Run(t, build, session.Config);
                result.Completed = DateTime.Now;
                session.Results.Add(result);
                await _uow.CommitAsync();
            }
        }
    }
}