using NPatterns;
using NPatterns.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class BuildProjectCommandHandler : IHandler<BuildProjectCommand>
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly ITestBuilder _builder;
        private readonly IEnumerable<ISourcePuller> _pullers;

        public BuildProjectCommandHandler(
            IMessageBus bus,
            IUnitOfWork uow,
            IEnumerable<ISourcePuller> pullers,
            ITestBuilder builder)
        {
            _bus = bus;
            _uow = uow;
            _pullers = pullers;
            _builder = builder;
        }

        #region IHandler<BuildProjectCommand> Members

        public void Handle(BuildProjectCommand message)
        {
            Task.WaitAll(HandleAsync(message));
        }

        public async Task HandleAsync(BuildProjectCommand message)
        {
            var project = message.Project;

            var puller = _pullers.FirstOrDefault(z => z.CanPull(project.RepoPathOrUrl));
            if (puller == null) throw new NotSupportedException("no puller for this project");

            //pull
            await puller.Pull(project.RepoPathOrUrl, project.WorkDir);

            //build
            var build = project.Build = await _builder.Build(project);

            _uow.Repository<TestProject>().Modify(project);
            await _uow.CommitAsync();

            await _bus.PublishAsync(new BuildProjectCompletedEvent { Project = project });
        }

        #endregion
    }
}
