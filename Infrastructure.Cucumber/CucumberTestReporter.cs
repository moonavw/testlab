using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestReporter : ITestReporter
    {
        private static readonly Regex RxFailed = new Regex(
            "<td class=\"failed\" colspan=\"\\d+\"><pre>(.+)</pre></td>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RxSummary = new Regex(
            "<p id=\"totals\">(.+)<br>(.+)</p>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _uow;

        public CucumberTestReporter(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool CanReport(TestSession session)
        {
            return session.Plan.Project.Type == TestProjectType.Cucumber;
        }

        public async Task Report(TestSession session)
        {
            //looking for result files, publish them to db
            var dir = new DirectoryInfo(Path.Combine(Constants.RESULT_ROOT, session.Name));
            var files = dir.GetFiles("*.html", SearchOption.AllDirectories);

            //TODO: read file async
            var q = (from f in files
                     let caseName = Path.GetFileNameWithoutExtension(f.Name)
                     let text = f.OpenText().ReadToEnd()
                     let summaryMatch = RxSummary.Match(text)
                     let failMatch = RxFailed.Match(text)
                     from r in session.Runs
                     where r.Case.Name.Equals(caseName, StringComparison.OrdinalIgnoreCase)
                     select new
                     {
                         run = r,
                         result = new TestResult
                         {
                             Reported = f.CreationTime,
                             File = f.FullName,
                             Summary = summaryMatch.Groups[1].Value + "\n" + summaryMatch.Groups[2].Value + (failMatch.Success ? "\n" + failMatch.Groups[1].Value : ""),
                             Type = failMatch.Success ? TestResultType.Fail : TestResultType.Pass
                         }
                     }).ToList();

            q.ForEach(z => z.run.Result = z.result);

            await _uow.CommitAsync();
        }
    }
}