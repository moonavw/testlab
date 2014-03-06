using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;
using NPatterns;
using NPatterns.Messaging;

namespace TestLab.Application
{
    public class StartTestSessionCommandHandler : IHandler<StartTestSessionCommand>
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IArchiver _archiver;

        public StartTestSessionCommandHandler(
            IMessageBus bus,
            IUnitOfWork uow,
            IArchiver archiver)
        {
            _bus = bus;
            _uow = uow;
            _archiver = archiver;
        }

        #region IHandler<StartTestSessionCommand> Members

        public void Handle(StartTestSessionCommand message)
        {
            Task.WaitAll(HandleAsync(message));
        }

        public async Task HandleAsync(StartTestSessionCommand message)
        {
            var session = message.Session;

            //start
            session.Started = DateTime.Now;

            //extract
            await _archiver.Extract(session.Build.Location, session.BuildDirOnAgent);

            //runs
            var runs = session.Runs;
            if (runs.Count == 0) throw new InvalidOperationException("no tests for this test session");

            var tasks = (from t in runs
                         select _bus.PublishAsync(new StartTestRunCommand { Run = t })).ToArray();

            await Task.WhenAll(tasks);

            session.Completed = DateTime.Now;
            _uow.Repository<TestSession>().Modify(session);
            await _uow.CommitAsync();
        }

        #endregion
    }
}
