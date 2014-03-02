using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.IO;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestProject : AggregateRoot, IAuditable
    {
        public TestProject()
        {
            Build = new TestBuild();
            Cases = new HashSet<TestCase>();
            Plans = new HashSet<TestPlan>();
            Sessions = new HashSet<TestSession>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DriverName { get; set; }

        [Required]
        [Display(Name = "Repository Path Or Url")]
        public string RepoPathOrUrl { get; set; }

        [Display(Name = "Build Output Path")]
        public string BuildOutputPath { get; set; }

        [Display(Name = "Build Script")]
        public string BuildScript { get; set; }

        public string WorkDir
        {
            get { return Path.Combine(Constants.PROJ_ROOT, ToString()); }
        }

        public string BuildOutputDir
        {
            get { return Path.Combine(WorkDir, BuildOutputPath ?? ""); }
        }

        public TestBuild Build { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        public virtual ICollection<TestSession> Sessions { get; set; }

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        public override string ToString()
        {
            return Name.Replace(" ", "");
        }
    }
}