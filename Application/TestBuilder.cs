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

        public async Task<TestBuild> Build(TestProject project)
        {
            string workDir = project.WorkDir;

            var build = new TestBuild
            {
                Started = DateTime.Now,
                Name = string.Format("{0}_{1:yyyyMMdd_hhmm}", project, DateTime.Now)
            };

            if (!string.IsNullOrEmpty(project.BuildScript))
            {
                var pi = new ProcessStartInfo(project.BuildScript) { WorkingDirectory = workDir };
                await ProcessEx.RunAsync(pi);
            }

            build.Completed = DateTime.Now;

            return build;
        }

        #endregion
    }
}