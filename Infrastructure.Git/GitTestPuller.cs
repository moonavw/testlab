using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RunProcessAsTask;
using TestLab.Domain;

namespace TestLab.Infrastructure.Git
{
    public class GitTestPuller : ITestPuller
    {
        private readonly IUnitOfWork _uow;

        public GitTestPuller(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool CanPull(TestProject project)
        {
            return project.Source.Type == TestSourceType.Git;
        }

        public async Task Pull(TestProject project)
        {
            //git clone or git pull
            var pi = !Directory.Exists(project.LocalPath)
                ? new ProcessStartInfo("git", string.Format("clone {0} {1}", project.Source.PathOrUrl, project.LocalPath))
                : new ProcessStartInfo("git", "pull") { WorkingDirectory = project.LocalPath };

            await ProcessEx.RunAsync(pi);

            project.Source.Pulled = DateTime.Now;

            await _uow.CommitAsync();
        }
    }
}