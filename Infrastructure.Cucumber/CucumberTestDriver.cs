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


        public async Task<TestResult> Run(TestCase test, TestBuild build, TestSession session)
        {
            //get test's feature file
            string workFile = Path.Combine(Constants.BUILD_ROOT, build.Name, test.Location);
            string outputFileName = Path.ChangeExtension(Path.Combine(session.Name, test.Name), ".html");
            string startProgram = @"c:\ruby200-x64\bin\cucumber.bat";
            string startProgramArgs = string.Format(@"{0} --tag @Name_{1} -f html --out {2}", workFile, test.Name, Path.Combine(Constants.RESULT_ROOT, outputFileName));
            var remoteResultFile = new FileInfo(Path.Combine(session.RemoteResultRoot, outputFileName));
            if (!remoteResultFile.Directory.Exists)
                remoteResultFile.Directory.Create();

            var result = new TestResult { Started = DateTime.Now, Case = test };

            using (TaskService ts = new TaskService(session.Server, session.UserName, session.Domain, session.Password))
            {
                string taskName = string.Format("{0}_{1}", session.Name, test.Name);
                var td = ts.NewTask();
                td.Actions.Add(new ExecAction(startProgram, startProgramArgs));
                var t = ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate,
                    session.DomainUser, session.Password, TaskLogonType.Password);
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
            result.Output = remoteResultFile.FullName;
            result.Summary = summaryMatch.Groups[1].Value + " " + summaryMatch.Groups[2].Value;
            result.PassOrFail = !failMatch.Success;
            if (result.PassOrFail == false)
            {
                foreach (Match m in RxFailDetails.Matches(failMatch.Groups[1].Value))
                {
                    result.Summary += "\n" + m.Groups[1].Value;
                }
            }
            result.Completed = DateTime.Now;

            return result;
        }

        #endregion
    }
}