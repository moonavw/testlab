using NPatterns.ObjectRelational;
using System;
using System.ComponentModel.DataAnnotations;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public abstract class TestJob : Entity, IAuditable, IStartable
    {
        public int Id { get; set; }

        public virtual TestAgent Agent { get; set; }

        #region IStartable Members

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }

        #endregion

        #region IAuditable Members

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion
    }

    public enum TestJobType
    {
        TestBuild,
        TestQueue
    }
}
