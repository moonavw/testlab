using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestLab.Domain;
using Microsoft.Win32.TaskScheduler;
using System.Threading;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestDriver : ITestDriver
    {
        private static readonly Regex RxTag = new Regex(@"@Name_(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RxFail = new Regex(
            "<td class=\"failed\" colspan=\"\\d+\">(.+)</td>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RxFailDetails = new Regex(
            "<pre>([^<]+)</pre>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RxSummary = new Regex(
            @"(\d+ scenarios [^<]*)<br />(\d+ steps \([^\)]+\))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Implementation of ITestDriver

        public string Name
        {
            get { return "Cucumber"; }
        }

        public async Task<IEnumerable<TestCase>> Publish(TestProject project)
        {
            string srcPath = project.WorkDir;
            //looking for test files, publish them to db
            var dir = new DirectoryInfo(srcPath);
            var files = dir.GetFiles("*.feature", SearchOption.AllDirectories);
            var tests = new List<TestCase>();
            foreach (var f in files)
            {
                string text = await f.OpenText().ReadToEndAsync();
                var matches = RxTag.Matches(text);
                tests.AddRange(from Match m in matches
                               select new TestCase
                               {
                                   Published = DateTime.Now,
                                   Name = m.Groups[1].Value,
                                   Location = f.FullName.Remove(0, srcPath.Length + 1)
                               });
            }

            return tests;
        }


        public async Task<TestResult> Run(TestRun run)
        {
            var test = run.Case;
            var session = run.Session;
            var build = session.Build;
            var agent = session.Agent;

            //get test's feature file
            string workFile = Path.Combine(build.Location, test.Location);
            string outputFileName = Path.ChangeExtension(test.Name, ".html");

            //find ruby folder for startProgram
            //const string startProgram = @"c:\ruby200-x64\bin\cucumber.bat";
            var rubyDir = new DirectoryInfo(string.Format(@"\\{0}\c$", agent.Server)).GetDirectories("ruby*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (rubyDir == null) throw new DirectoryNotFoundException("ruby installation not found on " + agent.Server);
            string startProgram = string.Format(@"c:\{0}\bin\cucumber.bat", rubyDir.Name);

            string startProgramArgs = string.Format(@"{0} --tag @Name_{1} -f html --out {2}", workFile, test.Name, Path.Combine(session.OutputDir, outputFileName));
            var remoteResultFile = new FileInfo(Path.Combine(session.OutputDirOnAgent, outputFileName));
            if (!remoteResultFile.Directory.Exists)
                remoteResultFile.Directory.Create();

            using (var ts = new TaskService(agent.Server, agent.UserName, agent.Domain, agent.Password))
            {
                string taskName = string.Format("{0}_{1}", session, test.Name);
                var td = ts.NewTask();
                td.Actions.Add(new ExecAction(startProgram, startProgramArgs));
                var t = ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate,
                    agent.DomainUser, agent.Password, TaskLogonType.Password);
                t.Run();
                do
                {
                    Thread.Sleep(1000);
                }
                while (t.State == TaskState.Running);
                ts.RootFolder.DeleteTask(taskName);
            }

            //parse result from output file
            string text = await remoteResultFile.OpenText().ReadToEndAsync();

            var summaryMatch = RxSummary.Match(text);
            var failMatch = RxFail.Match(text);

            var result = new TestResult
            {
                Output = remoteResultFile.FullName,
                Summary = summaryMatch.Groups[1].Value + " " + summaryMatch.Groups[2].Value,
                PassOrFail = !failMatch.Success
            };
            if (result.PassOrFail == false)
            {
                var q = from Match m in RxFailDetails.Matches(failMatch.Groups[1].Value)
                        select m.Groups[1].Value;
                result.ErrorDetails = string.Join("\n", q);
            }

            return result;
        }

        #endregion
    }
}