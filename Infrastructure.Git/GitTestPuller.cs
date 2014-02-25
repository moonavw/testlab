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
        #region Implementation of ITestPuller

        public TestRepoType Type
        {
            get { return TestRepoType.Git; }
        }

        public Task Pull(TestSrc src, TestRepo repo)
        {
            string pathOrUrl = repo.PathOrUrl;
            string workDir = src.Location;

            //git clone or git pull
            var pi = !Directory.Exists(workDir)
                ? new ProcessStartInfo("git", string.Format("clone {0} {1}", pathOrUrl, workDir))
                : new ProcessStartInfo("git", "pull") { WorkingDirectory = workDir };

            return ProcessEx.RunAsync(pi);
        }

        #endregion
    }
}