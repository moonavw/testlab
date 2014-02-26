using System;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : Entity
    {
        public int Id { get; set; }

        public string Name
        {
            get { return string.Format("{0}_{1:yyyyMMdd_hhmm}", Project.Name, Completed); }
        }

        public string ArchivePath
        {
            get { return Path.Combine(Constants.BUILD_ROOT, Name); }
        }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public DateTime? Archived { get; set; }

        public int TestProjectId { get; set; }

        public virtual TestProject Project { get; set; }
    }
}