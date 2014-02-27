using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RunProcessAsTask;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestDriver : ITestDriver
    {
        private static readonly Regex RxTag = new Regex(@"@Name_(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RxFailed = new Regex(
            "<td class=\"failed\" colspan=\"\\d+\"><pre>(.+)</pre></td>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RxSummary = new Regex(
            "<p id=\"totals\">(.+)<br>(.+)</p>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Implementation of ITestRunner

        public string Name
        {
            get { return "Cucumber"; }
        }

        public async Task<IEnumerable<TestCase>> Publish(TestBuild build)
        {
            string srcPath = build.Project.WorkDir;
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


        public async Task<TestResult> Run(TestCase test, TestBuild build, TestConfig config)
        {
            //TODO: get features's parent folder
            string workDir = Path.GetDirectoryName(Path.Combine(Constants.BUILD_ROOT, build.Name, test.Location));
            string outputFile = Path.ChangeExtension(Path.Combine(Constants.RESULT_ROOT, build.Name, test.Name), ".html");
            string startProgram = string.Format(@"cucumber --tag @Name_{0} -f html --out {1}", test.Name, outputFile);

            var result = new TestResult { Started = DateTime.Now };


            var pi = new ProcessStartInfo(Constants.RDP_CLIENT,
                                          string.Format(@"{0} {1} {2} {3} {4} {5}",
                                                        config.RdpServer,
                                                        config.RdpDomain,
                                                        config.RdpUserName,
                                                        config.RdpPassword,
                                                        workDir,
                                                        startProgram));

            //await ProcessEx.RunAsync(pi);
            Process.Start(pi);
            //parse result from output file
            string remoteResultFile = Path.ChangeExtension(Path.Combine(config.RemoteResultRoot, build.Name, test.Name), ".html");
            var file = new FileInfo(remoteResultFile);
            string text = await file.OpenText().ReadToEndAsync();

            var summaryMatch = RxSummary.Match(text);
            var failMatch = RxFailed.Match(text);
            result.File = file.FullName;
            result.Summary = summaryMatch.Groups[1].Value + "\n" + summaryMatch.Groups[2].Value + (failMatch.Success ? "\n" + failMatch.Groups[1].Value : "");
            result.PassOrFail = !failMatch.Success;
            result.Completed = DateTime.Now;

            return result;
        }

        #endregion
    }
}