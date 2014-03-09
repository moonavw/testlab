using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class PublishTestJob : IJob
    {
        private readonly IUnitOfWork _uow;
        private readonly IEnumerable<ITestDriver> _drivers;

        public PublishTestJob(
            IUnitOfWork uow,
            IEnumerable<ITestDriver> drivers)
        {
            _uow = uow;
            _drivers = drivers;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int projectId = dataMap.GetInt("TestProjectId");

            Trace.TraceInformation("Instance {0} of {1} for project {2}", key, GetType().Name, projectId);

            PublishTest(projectId).Wait();
        }

        #endregion

        private async Task PublishTest(int projectId)
        {
            var repo = _uow.Repository<TestProject>();
            var project = await repo.FindAsync(projectId);

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
            toAdd.ForEach(z => project.Cases.Add(z));

            repo.Modify(project);
            await _uow.CommitAsync();
        }
    }
}