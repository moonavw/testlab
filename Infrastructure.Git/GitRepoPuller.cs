using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RunProcessAsTask;

namespace TestLab.Infrastructure.Git
{
    public class GitRepoPuller : IRepoPuller
    {
        #region Implementation of ITestPuller

        public bool CanPull(string repoPathOrUrl)
        {
            return repoPathOrUrl.EndsWith(".git", StringComparison.OrdinalIgnoreCase);
        }

        public Task Pull(string repoPathOrUrl, string workDir)
        {
            //git clone or git pull
            var pi = !Directory.Exists(workDir)
                ? new ProcessStartInfo("git", string.Format("clone {0} {1}", repoPathOrUrl, workDir))
                : new ProcessStartInfo("git", "pull") { WorkingDirectory = workDir };

            return ProcessEx.RunAsync(pi);
        }

        #endregion
    }
}