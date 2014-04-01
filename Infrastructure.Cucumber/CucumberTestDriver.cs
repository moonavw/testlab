using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestLab.Domain;

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
            @"(\d+ scenario[^<]*)<br />(\d+ step[^\)]+\))",
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

        public TestRunTask CreateTask(TestRun run, TestAgent agent)
        {
            var test = run.Case;
            var session = run.Session;
            var build = session.Build;

            //get test's feature file
            string workFile = Path.Combine(build.Location, test.Location);
            string outputFileName = Path.ChangeExtension(test.Name, ".html");

            //find ruby folder for startProgram
            //const string startProgram = @"c:\ruby200-x64\bin\cucumber.bat";
            var rubyDir = new DirectoryInfo(string.Format(@"\\{0}\c$", agent.Server)).GetDirectories("ruby*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (rubyDir == null) throw new DirectoryNotFoundException("ruby installation not found on " + agent.Server);
            string startProgram = string.Format(@"c:\{0}\bin\cucumber.bat", rubyDir.Name);

            string startProgramArgs = string.Format(@"{0} --tag @Name_{1} -f html --out {2}", workFile, test.Name, Path.Combine(session.OutputDir, outputFileName));
            var remoteResultFile = new FileInfo(Path.Combine(agent.GetOutputDir(session), outputFileName));
            if (!remoteResultFile.Directory.Exists)
                remoteResultFile.Directory.Create();

            return new TestRunTask
            {
                Run = run,
                Agent = agent,
                StartProgram = startProgram,
                StartProgramArgs = startProgramArgs,
                OutputFile = remoteResultFile.FullName
            };
        }

        public async Task<TestResult> ParseResult(TestRunTask task)
        {
            var remoteResultFile = new FileInfo(task.OutputFile);
            if (!remoteResultFile.Exists)
                throw new FileNotFoundException("result file not found", remoteResultFile.FullName);

            //parse result from output file
            string text = null;
            int attempts = 0;
            while (text == null && attempts++ < 5)
            {
                try
                {
                    using (var sr = remoteResultFile.OpenText())
                    {
                        text = await sr.ReadToEndAsync();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("error when parsing result file, retry: {0}, error: {1}", attempts, ex.ToString());
                }
            }
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