using System.Collections.Generic;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain.Models
{
    public class TestSource : Entity
    {
        public TestSource()
        {
            TestBuilds = new HashSet<TestBuild>();
            TestCases = new HashSet<TestCase>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string SourcePath { get; set; }

        public SourceType Type { get; set; }

        public virtual ICollection<TestBuild> TestBuilds { get; set; }

        public virtual ICollection<TestCase> TestCases { get; set; }

        public string LocalPath
        {
            get { return string.IsNullOrEmpty(Name) ? null : Path.Combine(Constants.SRC_ROOT, Name); }
        }
    }
}