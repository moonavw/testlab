using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class ArchiveBuildJob : IJob
    {
        private readonly IUnitOfWork _uow;
        private readonly IArchiver _archiver;

        public ArchiveBuildJob(
            IUnitOfWork uow,
            IArchiver archiver)
        {
            _uow = uow;
            _archiver = archiver;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int projectId = dataMap.GetInt("TestProjectId");

            Trace.TraceInformation("Instance {0} of {1} for project {2}", key, GetType().Name, projectId);

            ArchiveBuild(projectId).Wait();
        }

        #endregion

        private async Task ArchiveBuild(int projectId)
        {
            var repo = _uow.Repository<TestProject>();
            var project = await repo.FindAsync(projectId);
            var build = project.Build;

            //archive
            await _archiver.Archive(project.BuildOutputDir, build.Location);
        }
    }
}