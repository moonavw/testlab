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
            Builds = new HashSet<TestBuild>();
            Cases = new HashSet<TestCase>();
            Plans = new HashSet<TestPlan>();
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
            get { return Path.Combine(Constants.PROJ_ROOT, Name); }
        }

        public string BuildOutputDir
        {
            get { return Path.Combine(WorkDir, BuildOutputPath ?? ""); }
        }

        public TestBuild LastBuild
        {
            get { return Builds.LastOrDefault(z => z.Completed != null && z.Archived != null); }
        }

        public virtual ICollection<TestBuild> Builds { get; set; }

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