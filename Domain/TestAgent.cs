using NPatterns.ObjectRelational;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestAgent : Entity, IArchivable
    {
        public TestAgent()
        {
            Jobs = new HashSet<TestJob>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? LastTalk { get; set; }

        public virtual ICollection<TestJob> Jobs { get; set; }

        public bool IsOnline
        {
            get
            {
                if (LastTalk == null)
                    return false;
                return LastTalk.Value.AddSeconds(Constants.AGENT_KEEPALIVE) >= DateTime.Now;
            }
        }

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