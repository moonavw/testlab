using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RunProcessAsTask;
using TestLab.Domain;
using TestLab.Infrastructure;

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
                Name = string.Format("{0}_{1:yyyyMMdd_hhmm}", project.Name, DateTime.Now)
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