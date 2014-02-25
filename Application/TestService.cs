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
        private readonly IEnumerable<ITestBuilder> _builders;
        private readonly ITestDeployer _deployer;
        private readonly IEnumerable<ITestPublisher> _publishers;
        private readonly IEnumerable<ITestPuller> _pullers;
        private readonly IEnumerable<ITestRunner> _runners;
        private readonly IUnitOfWork _uow;

        public TestService(
            IEnumerable<ITestPuller> pullers,
            IEnumerable<ITestBuilder> builders,
            IEnumerable<ITestPublisher> publishers,
            IEnumerable<ITestRunner> runners,
            ITestDeployer deployer,
            IUnitOfWork uow)
        {
            _pullers = pullers;
            _builders = builders;
            _publishers = publishers;
            _runners = runners;
            _deployer = deployer;
            _uow = uow;
        }

        public async Task Build(TestProject project)
        {
            var puller = _pullers.FirstOrDefault(z => z.Type == project.Repo.Type);
            if (puller == null) throw new NotSupportedException();
            var builder = _builders.FirstOrDefault(z => z.Type == project.Src.Type);
            if (builder == null) throw new NotSupportedException();
            var publisher = _publishers.FirstOrDefault(z => z.Type == project.Bin.Type);
            if (publisher == null) throw new NotSupportedException();

            //pull
            project.Src.Location = Path.Combine(Constants.SRC_ROOT, project.Name);
            await puller.Pull(project.Src, project.Repo);
            project.Src.Pulled = DateTime.Now;
            await _uow.CommitAsync();

            //build
            await builder.Build(project.Src, project.Bin);
            project.Bin.Built = DateTime.Now;
            await _uow.CommitAsync();

            //publish
            var tests = (await publisher.Publish(project.Bin)).ToList();
            var toDel = project.Cases.ExceptBy(tests, z => z.FullName + "#" + z.Name).ToList();
            var toAdd = tests.ExceptBy(project.Cases, z => z.FullName + "#" + z.Name).ToList();

            toDel.ForEach(z => project.Cases.Remove(z));
            toAdd.ForEach(z =>
            {
                z.Published = DateTime.Now;
                project.Cases.Add(z);
            });
            await _uow.CommitAsync();
        }

        public async Task Run(TestSession session)
        {
            session.Bin.Built = session.Plan.Project.Bin.Built;

            var runner = _runners.FirstOrDefault(z => z.Type == session.Bin.Type);
            if (runner == null) throw new NotSupportedException();

            //deploy
            session.Bin.Location = Path.Combine(Constants.BIN_ROOT, session.Plan.Project.Name, (session.Bin.Built ?? DateTime.Now).ToString(Constants.DATETIME_FORMAT));
            await _deployer.Deploy(session.Plan.Project.Bin, session.Bin);

            //run
            session.Results.Clear();
            foreach (var t in session.Plan.Cases)
            {
                var result = await runner.Run(t, session.Bin, session.Config);
                result.Completed = DateTime.Now;
                session.Results.Add(result);
                await _uow.CommitAsync();
            }
        }
    }
}