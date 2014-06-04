using RunProcessAsTask;
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
        private static readonly Regex RxTagName = new Regex(@"@Name_([\w\-_\.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RxTagKeyword = new Regex(@"@Keyword_([\w\-_,#]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

        public async Task<IEnumerable<TestCase>> Publish(TestBuild build)
        {
            var project = build.Project;
            string srcPath = project.SrcDir;
            //looking for test files, publish them to db
            var dir = new DirectoryInfo(srcPath);
            var files = dir.GetFiles("*.feature", SearchOption.AllDirectories);
            var tests = new List<TestCase>();
            foreach (var f in files)
            {
                string text = await f.OpenText().ReadToEndAsync();
                var matches = RxTagName.Matches(text);
                foreach (Match m in matches)
                {
                    int start = m.Index < 100 ? 0 : m.Index - 100;
                    int end = m.Index + 100 > text.Length - 1 ? m.Index + 100 : text.Length - 1;
                    var t = new TestCase
                    {
                        Name = m.Groups[1].Value,
                        Location = f.FullName.Remove(0, srcPath.Length + 1)
                    };
                    var km = RxTagKeyword.Match(text.Substring(start, end - start));
                    if (km.Success)
                    {
                        t.Keyword = km.Groups[1].Value;
                    }
                    tests.Add(t);
                }
            }

            return tests;
        }

        public async Task<TestResult> Run(TestRun run)
        {
            var test = run.Case;
            var agent = run.Queue.Agent;
            var session = run.Queue.Session;
            var build = session.Build;

            //get test's feature file
            string workFile = Path.Combine(build.LocalPath, test.Location);
            string outputFileName = Path.ChangeExtension(test.Name, ".html");
            var outputFile = new FileInfo(Path.Combine(session.LocalPath, outputFileName));
            if (!outputFile.Directory.Exists)
                outputFile.Directory.Create();

            //find ruby folder for startProgram
            //const string startProgram = @"c:\ruby200-x64\bin\cucumber.bat";
            var rubyDir = new DirectoryInfo(string.Format(@"\\{0}\c$", agent.Name)).GetDirectories("ruby*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (rubyDir == null) throw new DirectoryNotFoundException("ruby installation not found on " + agent.Name);
            string startProgram = string.Format(@"c:\{0}\bin\cucumber.bat", rubyDir.Name);
            string startProgramArgs = string.Format(@"{0} --tag @Name_{1} -f html --out {2}", workFile, test.Name, outputFile.FullName);

            var pi = new ProcessStartInfo(startProgram, startProgramArgs);
            pi.EnvironmentVariables.Add(session.Config.Key, session.Config.Value);
            await ProcessEx.RunAsync(pi);

            outputFile.Refresh();
            if (!outputFile.Exists)
                throw new FileNotFoundException("result file not found", outputFile.FullName);

            //parse result from output file
            string text = null;
            int attempts = 0;
            while (text == null && attempts++ < 5)
            {
                try
                {
                    using (var sr = outputFile.OpenText())
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
                Output = Path.Combine(session.GetPathOnAgent(agent), outputFileName),
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