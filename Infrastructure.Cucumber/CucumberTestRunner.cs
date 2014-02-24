using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zip;
using RunProcessAsTask;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestRunner : ITestRunner
    {
        private readonly IUnitOfWork _uow;

        public CucumberTestRunner(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool CanRun(TestSession session)
        {
            return session.Plan.Project.Type == TestProjectType.Cucumber;
        }

        public async Task Run(TestSession session)
        {
            session.Started = DateTime.Now;

            await _uow.CommitAsync();

            //copy build to target
            //TODO: unzip files async
            using (var zip = ZipFile.Read(session.Plan.Project.Build.PublishPath))
            {
                zip.ExtractAll(Constants.RUN_ROOT);
            }

            //generate run.cmd by testplan
            string cmdFile = Path.ChangeExtension(session.Name, ".cmd");
            using (var sw = File.CreateText(Path.Combine(Constants.RUN_ROOT, cmdFile)))
            {
                sw.WriteLine(@"cd {0}", session.Plan.Project.Build.Name);
                foreach (var t in session.Runs)
                {
                    sw.WriteLine("pushd {0}", t.Case.DirectoryName);
                    sw.WriteLine(@"cucumber --tag @Name_{0} -f html --out {1}\{2}\{0}.html", t.Case.Name,
                        Constants.RESULT_ROOT, session.Name);
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
                    session.Config.RdpServer,
                    session.Config.RdpDomain,
                    session.Config.RdpUserName,
                    session.Config.RdpPassword,
                    Constants.RDP_START,
                    cmdFile));

            session.Runs.ToList().ForEach(z => z.Started = DateTime.Now);

            await ProcessEx.RunAsync(pi);

            session.Runs.ToList().ForEach(z => z.Completed = DateTime.Now);

            session.Completed = DateTime.Now;

            await _uow.CommitAsync();
        }
    }
}