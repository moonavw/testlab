using System;
using System.Collections.Generic;
using System.Linq;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestReport : Entity
    {
        public TestReport()
        {
            TestResults = new HashSet<TestResult>();
        }

        public DateTime Created { get; set; }

        public int TestRunId { get; set; }

        public virtual TestRun TestRun { get; set; }

        public virtual ICollection<TestResult> TestResults { get; set; }

        public ResultType Type
        {
            get
            {
                if (TestResults.Any(z => z.Type == ResultType.None))
                    return ResultType.None;
                if (TestResults.Any(z => z.Type == ResultType.Fail))
                    return ResultType.Fail;
                return ResultType.Pass;
            }
        }

        public string Summary
        {
            get
            {
                return string.Format("{0} pass, {1} failed, {2} waiting",
                    TestResults.Count(z => z.Type == ResultType.Pass),
                    TestResults.Count(z => z.Type == ResultType.Fail),
                    TestResults.Count(z => z.Type == ResultType.None));
            }
        }
    }
}