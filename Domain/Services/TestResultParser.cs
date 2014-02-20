using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Domain.Models;
using System.IO;
using TestLab.Infrastructure;
using System.Text.RegularExpressions;

namespace TestLab.Domain.Services
{
    public interface ITestResultParser
    {
        IEnumerable<TestResult> Parse(TestRun run);
    }

    public class CucumberTestResultParser : ITestResultParser
    {
        private static readonly Regex RxFailed = new Regex("<td class=\"failed\" colspan=\"\\d+\"><pre>(.+)</pre></td>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RxSummary = new Regex("<p id=\"totals\">(.+)<br>(.+)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _uow;

        public CucumberTestResultParser(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IEnumerable<TestResult> Parse(TestRun run)
        {
            //looking for result files, publish them to db
            var dir = new DirectoryInfo(Path.Combine(Constants.RESULT_ROOT, run.Name));
            var files = dir.GetFiles("*.html", SearchOption.AllDirectories);

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

            q.ForEach(z => run.TestResults.Add(z));

            _uow.Commit();

            return q;
        }
    }
}
