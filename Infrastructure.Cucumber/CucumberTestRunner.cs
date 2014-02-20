using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using TestLab.Domain;
using TestLab.Infrastructure;
using TestLab.Domain.Services;
using RunProcessAsTask;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestRunner : ITestRunner
    {
        private readonly IUnitOfWork _uow;

        public CucumberTestRunner(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public TestRun Run(TestPlan plan, TestBuild build)
        {
            return RunAsync(plan, build).Result;
        }


        public async Task<TestRun> RunAsync(TestPlan plan, TestBuild build)
        {
            //create run
            var run = new TestRun
            {
                Created = DateTime.Now,
                TestBuild = build
            };
            plan.TestRuns.Add(run);
            _uow.Commit();

            //copy build to target
            //TODO: unzip files async
            using (var zip = ZipFile.Read(build.LocalPath))
            {
                zip.ExtractAll(Constants.RUN_ROOT);
            }

            //generate run.cmd by testplan
            string cmdFile = Path.ChangeExtension(run.Name, ".cmd");
            using (var sw = File.CreateText(Path.Combine(Constants.RUN_ROOT, cmdFile)))
            {
                sw.WriteLine(@"cd {0}", build.Name);
                foreach (var t in plan.TestCases)
                {
                    sw.WriteLine("pushd {0}", t.DirectoryName);
                    sw.WriteLine(@"cucumber --tag @Name_{0} -f html --out {1}\{2}\{0}.html", t.Name,
                        Constants.RESULT_ROOT, run.Name);
                    sw.WriteLine("popd");
                }
            }

            //rdp
            string startFile = Path.Combine(Constants.RUN_ROOT, Path.GetFileName(Constants.RDP_START));
            if (!File.Exists(startFile))
            {
                File.WriteAllText(startFile, "cd/d %~dp0 \r\n %*");
            }
            var pi = new ProcessStartInfo(Constants.RDP_TOOL,
                string.Format(@"{0} {1} {2} {3} {4} {5}",
                    Constants.RDP_SERVER,
                    Constants.RDP_DOMAIN,
                    Constants.RDP_USER,
                    Constants.RDP_PWD,
                    Constants.RDP_START,
                    cmdFile));

            await ProcessEx.RunAsync(pi);

            return run;
        }
    }
}