using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestResult : Entity
    {
        public int TestRunId { get; set; }

        public int TestCaseId { get; set; }

        public string LogFile { get; set; }

        public string Summary { get; set; }

        public string ErrorMessage { get; set; }

        public ResultType Type { get; set; }

        public DateTime Created { get; set; }

        public virtual TestReport TestReport { get; set; }

        public virtual TestCase TestCase { get; set; }
    }
}