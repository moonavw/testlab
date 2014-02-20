using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TestLab.Domain;
using TestLab.Domain.Models;
using TestLab.Domain.Services;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestReporter : ITestReporter
    {
        private static readonly Regex RxFailed = new Regex(
            "<td class=\"failed\" colspan=\"\\d+\"><pre>(.+)</pre></td>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RxSummary = new Regex("<p id=\"totals\">(.+)<br>(.+)</p>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _uow;

        public CucumberTestReporter(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public TestReport Report(TestRun run)
        {
            //looking for result files, publish them to db
            var dir = new DirectoryInfo(Path.Combine(Constants.RESULT_ROOT, run.Name));
            var files = dir.GetFiles("*.html", SearchOption.AllDirectories);

            //TODO: read file async
            var q = (from f in files
                     let text = f.OpenText().ReadToEnd()
                     let summaryMatch = RxSummary.Match(text)
                     let failMatch = RxFailed.Match(text)
                     select new TestResult
                     {
                         Created = f.CreationTime,
                         LogFile = f.FullName,
                         Summary = summaryMatch.Groups[1].Value + "\n" + summaryMatch.Groups[2].Value,
                         ErrorMessage = failMatch.Success ? "\n" + failMatch.Groups[1].Value : "",
                         Type = failMatch.Success ? ResultType.Fail : ResultType.Pass
                     }).ToList();

            var report = new TestReport
            {
                Created = DateTime.Now
            };
            q.ForEach(z => report.TestResults.Add(z));

            run.TestReport = report;

            _uow.Commit();

            return report;
        }
    }
}