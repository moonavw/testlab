using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestPlan : Entity, IAuditable, IArchivable
    {
        public TestPlan()
        {
            Cases = new HashSet<TestCase>();
        }

        public int Id { get; set; }

        [RegularExpression("[a-zA-Z0-9 ]+")]
        [Required]
        public string Name { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        #region IAuditable Members

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        #region IArchivable Members

        public DateTime? Deleted { get; set; }
        public string DeletedBy { get; set; }

        #endregion

        public void SetCases(IEnumerable<TestCase> cases)
        {
            Cases.Clear();
            Cases = new HashSet<TestCase>(cases.Actives());
        }
    }
}