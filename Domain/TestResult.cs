using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestResult : Entity
    {
        public int TestCaseId { get; set; }

        public int TestSessionId { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public string File { get; set; }

        public string Summary { get; set; }

        public bool? PassOrFail { get; set; }

        public virtual TestCase Case { get; set; }

        public virtual TestSession Session { get; set; }
    }
}