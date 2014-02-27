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
            //get features's parent folder
            string workDir = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(Constants.BUILD_ROOT, build.Name, test.Location))).Parent.FullName;
            string outputFileName = Path.ChangeExtension(Path.Combine(build.Name, test.Name), ".html");
            string startProgram = @"c:\ruby200-x64\bin\cucumber.bat";
            string startProgramArgs = string.Format(@"--tag @Name_{0} -f html --out {1}", test.Name, Path.Combine(Constants.RESULT_ROOT, outputFileName));
            var remoteResultFile = new FileInfo(Path.Combine(config.RemoteResultRoot, outputFileName));
            if (!remoteResultFile.Directory.Exists)
                remoteResultFile.Directory.Create();

            var result = new TestResult { Started = DateTime.Now, Case = test };


            //var pi = new ProcessStartInfo(Constants.RDP_CLIENT,
            //                              string.Format(@"{0} {1} {2} {3} {4} {5} {6}",
            //                                            config.RdpServer,
            //                                            config.RdpDomain,
            //                                            config.RdpUserName,
            //                                            config.RdpPassword,
            //                                            workDir,
            //                                            startProgram, startProgramArgs));

            //debug
            var pi = new ProcessStartInfo
            {
                WorkingDirectory = workDir,
                FileName = startProgram,
                Arguments = startProgramArgs
            };

            await ProcessEx.RunAsync(pi);

            //parse result from output file
            string text = await remoteResultFile.OpenText().ReadToEndAsync();

            var summaryMatch = RxSummary.Match(text);
            var failMatch = RxFail.Match(text);
            result.File = remoteResultFile.FullName;
            result.Summary = summaryMatch.Groups[1].Value + summaryMatch.Groups[2].Value;            
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