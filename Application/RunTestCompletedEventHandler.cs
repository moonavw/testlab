using System;
using System.Linq;
using System.Threading.Tasks;
using NPatterns;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class RunTestCompletedEventHandler : IHandler<RunTestCompletedEvent>
    {
        private readonly IUnitOfWork _uow;

        public RunTestCompletedEventHandler(
            IUnitOfWork uow)
        {
            _uow = uow;
        }

        #region IHandler<RunTestCompletedEvent> Members

        public void Handle(RunTestCompletedEvent message)
        {
            HandleAsync(message).Wait();
        }

        public async Task HandleAsync(RunTestCompletedEvent message)
        {
            var repo = _uow.Repository<TestSession>();
            var session = await repo.FindAsync(message.TestSessionId);

            if (session.Runs.All(z => z.Completed != null))//&& z.Completed > session.Started
            {
                session.Completed = DateTime.Now;
                repo.Modify(session);
                await _uow.CommitAsync();
            }
        }

        #endregion
    }
}