using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestPlan : Entity, IAuditable
    {
        public TestPlan()
        {
            Cases = new HashSet<TestCase>();
            Sessions = new HashSet<TestSession>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int TestProjectId { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        public virtual ICollection<TestSession> Sessions { get; set; }

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion
    }
}