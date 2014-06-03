using NPatterns.ObjectRelational;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestCase : Entity, IAuditable, IArchivable
    {
        public TestCase()
        {
            Plans = new HashSet<TestPlan>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string Keyword { get; set; }

        public string FullName
        {
            get { return Location + "." + Name; }
        }

        public DateTime? Published
        {
            get { return Updated ?? Created; }
        }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

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

        public override string ToString()
        {
            return FullName;
        }
    }
}