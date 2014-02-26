using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RunProcessAsTask;
using TestLab.Domain;

namespace TestLab.Application
{
    public class TestBuilder : ITestBuilder
    {
        #region Implementation of ITestBuilder

        public async Task Build(TestBuild build)
        {
            string buildScript = build.Project.BuildScript;
            string workDir = build.Project.WorkDir;
            var pi = new ProcessStartInfo(buildScript) { WorkingDirectory = workDir };
            build.Started = DateTime.Now;
            await ProcessEx.RunAsync(pi);
            build.Completed = DateTime.Now;
        }

        #endregion
    }
}