using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Application
{
    public class TestService : ITestService
    {
        private readonly IEnumerable<ITestPuller> _pullers;
        private readonly IEnumerable<ITestBuilder> _builders;
        private readonly IEnumerable<ITestPublisher> _publishers;
        private readonly IEnumerable<ITestRunner> _runners;
        private readonly IEnumerable<ITestReporter> _reporters;

        public TestService(
            IEnumerable<ITestPuller> pullers,
            IEnumerable<ITestBuilder> builders,
            IEnumerable<ITestPublisher> publishers,
            IEnumerable<ITestRunner> runners,
            IEnumerable<ITestReporter> reporters
            )
        {
            _pullers = pullers;
            _builders = builders;
            _publishers = publishers;
            _runners = runners;
            _reporters = reporters;
        }

        #region Implementation of ITestService

        public async Task Build(TestProject project)
        {
            var puller = _pullers.FirstOrDefault(z => z.CanPull(project));
            if (puller == null) throw new NotSupportedException();
            var builder = _builders.FirstOrDefault(z => z.CanBuild(project));
            if (builder == null) throw new NotSupportedException();
            var publisher = _publishers.FirstOrDefault(z => z.CanPublish(project));
            if (publisher == null) throw new NotSupportedException();

            await puller.Pull(project);
            await builder.Build(project);
            await publisher.Publish(project);
        }

        public async Task Run(TestSession session)
        {
            var runner = _runners.FirstOrDefault(z => z.CanRun(session));
            if (runner == null) throw new NotSupportedException();
            var reporter = _reporters.FirstOrDefault(z => z.CanReport(session));
            if (reporter == null) throw new NotSupportedException();

            await runner.Run(session);
            await reporter.Report(session);
        }

        #endregion
    }
}