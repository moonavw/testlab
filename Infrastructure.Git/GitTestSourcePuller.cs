using System.Diagnostics;
using System.IO;
using RunProcessAsTask;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Infrastructure.Git
{
    public class GitTestSourcePuller : ITestSourcePuller
    {
        public void Pull(TestSource src)
        {
            //git clone or git pull
            var pi = !Directory.Exists(src.LocalPath)
                ? new ProcessStartInfo("git", string.Format("clone {0} {1}", src.SourcePath, src.LocalPath))
                : new ProcessStartInfo("git", "pull") { WorkingDirectory = src.LocalPath };

            ProcessEx.RunAsync(pi);
        }
    }
}