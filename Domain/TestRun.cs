using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestRun : Entity
    {
        public TestRun()
        {
            Result = new TestResult();
        }

        public int TestCaseId { get; set; }

        public int TestSessionId { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public TestResult Result { get; set; }

        public virtual TestCase Case { get; set; }

        public virtual TestSession Session { get; set; }
    }
}