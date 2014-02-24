using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestResult : ValueObject
    {
        public string File { get; set; }

        public string Summary { get; set; }

        public TestResultType Type { get; set; }

        public DateTime? Reported { get; set; }
    }
}