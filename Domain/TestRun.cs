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

        public TestResult Result { get; set; }

        public int TestCaseId { get; set; }

        public int TestQueueId { get; set; }

        public virtual TestCase Case { get; set; }

        public virtual TestQueue Queue { get; set; }

        #region IStartable Members

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }

        #endregion
    }
}