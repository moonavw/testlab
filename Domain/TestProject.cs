using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.IO;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestProject : AggregateRoot, IAuditable, IArchivable
    {
        public TestProject()
        {
            Cases = new HashSet<TestCase>();
            Plans = new HashSet<TestPlan>();
            Sessions = new HashSet<TestSession>();
            Builds = new HashSet<TestBuild>();
        }

        public int Id { get; set; }

        [RegularExpression("[a-zA-Z0-9 ]+")]
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

        public virtual ICollection<TestBuild> Builds { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public virtual ICollection<TestCase> Cases { get; set; }

        public virtual ICollection<TestSession> Sessions { get; set; }

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

        public string LocalPath
        {
            get { return Path.Combine(Constants.LOCAL_ROOT, ToString()); }
        }

        public string SrcDir
        {
            get { return Path.Combine(LocalPath, Constants.SRC_DIR_NAME); }
        }

        public string BuildOutputDir
        {
            get { return Path.Combine(SrcDir, BuildOutputPath ?? ""); }
        }

        public string GetPathOnAgent(TestAgent agent)
        {
            return Path.Combine(string.Format(Constants.AGENT_ROOT_FORMAT, agent.Name), ToString());
        }

        public override string ToString()
        {
            return Name.Replace(" ", "");
        }
    }
}