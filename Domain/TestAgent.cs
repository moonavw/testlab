using NPatterns.ObjectRelational;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestAgent : AggregateRoot, IArchivable
    {
        public TestAgent()
        {
            Jobs = new HashSet<TestJob>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? LastTalked { get; set; }

        public virtual ICollection<TestJob> Jobs { get; set; }

        public bool IsOnline
        {
            get
            {
                if (LastTalked == null)
                    return false;
                if (RunningCount > 0)
                    return true;
                return LastTalked.Value.AddSeconds(Constants.AGENT_KEEPALIVE) >= DateTime.Now;
            }
        }

        #region info

        public int CompletedCount
        {
            get { return Jobs.Count(z => z.Completed != null); }
        }

        public int RunningCount
        {
            get { return Jobs.Count(z => z.Started != null && z.Completed == null); }
        }

        public int NotStartedCount
        {
            get { return Jobs.Count(z => z.Started == null); }
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

        #region IArchivable Members

        public DateTime? Deleted { get; set; }
        public string DeletedBy { get; set; }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}