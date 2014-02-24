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
            Source = new TestSource();
            Build = new TestBuild();
            Cases = new HashSet<TestCase>();
            Plans = new HashSet<TestPlan>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public TestProjectType Type { get; set; }

        public TestSource Source { get; set; }

        public TestBuild Build { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        public string LocalPath
        {
            get { return string.IsNullOrEmpty(Name) ? null : Path.Combine(Constants.SRC_ROOT, Name); }
        }
    }
}