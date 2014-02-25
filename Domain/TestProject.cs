using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestProject : Entity, IAuditable
    {
        public TestProject()
        {
            Cases = new HashSet<TestCase>();
            Plans = new HashSet<TestPlan>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public TestRepo Repo { get; set; }

        public TestSrc Src { get; set; }

        public TestBin Bin { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion
    }
}