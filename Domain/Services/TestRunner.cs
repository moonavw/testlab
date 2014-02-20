using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Domain.Models;
using TestLab.Infrastructure;
using Ionic.Zip;
using System.IO;

namespace TestLab.Domain.Services
{
    public interface ITestRunner
    {
        TestRun Run(TestPlan plan, TestBuild build);
    }

    public class CucumberTestRunner : ITestRunner
    {
        private readonly IUnitOfWork _uow;

        public CucumberTestRunner(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public TestRun Run(TestPlan plan, TestBuild build)
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
            using (var zip = ZipFile.Read(build.LocalPath))
            {
                zip.ExtractAll(Constants.RUN_ROOT);
            }

            //generate run.cmd by testplan
            var sb = new StringBuilder();
            sb.AppendFormat(@"cd {0}", build.Name);
            sb.AppendLine();
            foreach (var t in plan.TestCases)
            {
                sb.AppendFormat("pushd {0}", t.DirectoryName);
                sb.AppendLine();
                sb.AppendFormat(@"cucumber --tag @Name_{0} -f html --out {1}\{2}\{0}.html", t.Name, Constants.RESULT_ROOT, run.Name);
                sb.AppendLine();
                sb.AppendLine("popd");
            }
            string cmdFile = Path.ChangeExtension(run.Name, ".cmd");
            File.WriteAllText(Path.Combine(Constants.RUN_ROOT, cmdFile), sb.ToString());

            //rdp
            string startFile = Path.Combine(Constants.RUN_ROOT, Path.GetFileName(Constants.RDP_START));
            if (!File.Exists(startFile))
            {
                File.WriteAllText(startFile, "cd/d %~dp0 \r\n %*");
            }
            var p = Process.Start(Constants.RDP_TOOL,
                string.Format(@"{0} {1} {2} {3} {4} {5}",
                Constants.RDP_SERVER,
                Constants.RDP_DOMAIN,
                Constants.RDP_USER,
                Constants.RDP_PWD,
                Constants.RDP_START,
                cmdFile));

            //TODO: mark completed on exit
            //p.Exited+=

            return run;
        }
    }
}
