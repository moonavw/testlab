using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestRun : Entity, IStartable
    {
        public TestRun()
        {
            Result = new TestResult();
        }

        public int TestCaseId { get; set; }

        public int TestSessionId { get; set; }

        public TestResult Result { get; set; }

        public virtual TestCase Case { get; set; }

        public virtual TestSession Session { get; set; }

        #region Implementation of IStartable

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }

        #endregion
    }
}