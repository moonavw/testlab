using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public class TestQueue : TestJob
    {
        public TestQueue()
        {
            Runs = new HashSet<TestRun>();
        }

        public virtual TestSession Session { get; set; }

        public virtual ICollection<TestRun> Runs { get; set; }

        #region info

        public int CompletedCount
        {
            get { return Runs.Count(z => z.Completed != null); }
        }

        public int RunningCount
        {
            get { return Runs.Count(z => z.Started != null && z.Completed == null); }
        }

        public int NotStartedCount
        {
            get { return Runs.Count(z => z.Started == null); }
        }

        public string Summary
        {
            get
            {
                return string.Format("{0} completed, {1} running, {2} not started",
                    CompletedCount,
                    RunningCount,
                    NotStartedCount);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} on {1}", Session, Agent);
        }
    }
}
