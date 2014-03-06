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
    public class StartTestRunCommandHandler : IHandler<StartTestRunCommand>
    {
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IEnumerable<ITestDriver> _drivers;

        public StartTestRunCommandHandler(
            IMessageBus bus,
            IUnitOfWork uow,
            IEnumerable<ITestDriver> drivers)
        {
            _bus = bus;
            _uow = uow;
            _drivers = drivers;
        }

        #region IHandler<StartTestRunCommand> Members

        public void Handle(StartTestRunCommand message)
        {
            Task.WaitAll(HandleAsync(message));
        }

        public async Task HandleAsync(StartTestRunCommand message)
        {
            var run = message.Run;
            var session = run.Session;
            var project = session.Project;
            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test session");

            run.Started = DateTime.Now;
            run.Result = await driver.Run(run);
            run.Completed = DateTime.Now;

            _uow.Repository<TestRun>().Modify(run);
            await _uow.CommitAsync();
        }

        #endregion
    }
}
