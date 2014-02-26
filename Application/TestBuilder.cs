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
            build.Started = DateTime.Now;
            string buildScript = build.Project.BuildScript;
            if (!string.IsNullOrEmpty(buildScript))
            {
                string workDir = build.Project.WorkDir;
                var pi = new ProcessStartInfo(buildScript) { WorkingDirectory = workDir };
                await ProcessEx.RunAsync(pi);
            }
            build.Completed = DateTime.Now;
        }

        #endregion
    }
}