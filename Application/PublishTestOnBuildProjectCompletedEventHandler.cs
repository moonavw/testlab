using MoreLinq;
using NPatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class PublishTestOnBuildProjectCompletedEventHandler : IHandler<BuildProjectCompletedEvent>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEnumerable<ITestDriver> _drivers;

        public PublishTestOnBuildProjectCompletedEventHandler(
            IUnitOfWork uow,
            IEnumerable<ITestDriver> drivers)
        {
            _uow = uow;
            _drivers = drivers;
        }

        #region IHandler<BuildProjectCompletedEvent> Members

        public void Handle(BuildProjectCompletedEvent message)
        {
            Task.WaitAll(HandleAsync(message));
        }

        public async Task HandleAsync(BuildProjectCompletedEvent message)
        {
            var project = message.Project;

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
            toAdd.ForEach(z =>
            {
                project.Cases.Add(z);
            });

            _uow.Repository<TestProject>().Modify(project);
            await _uow.CommitAsync();
        }

        #endregion
    }
}
