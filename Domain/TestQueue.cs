using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public class TestQueue : TestJob
    {
        public virtual TestSession Session { get; set; }

        public IEnumerable<TestRun> Runs
        {
            get
            {
                return from e in Session.Runs
                       where e.Agent.Id == Agent.Id
                       select e;
            }
        }

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
    }
}
