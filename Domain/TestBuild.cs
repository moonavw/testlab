using System;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : Entity
    {
        public int Id { get; set; }

        public int TestSourceId { get; set; }

        public DateTime Created { get; set; }

        public virtual TestSource TestSource { get; set; }

        public string Name
        {
            get { return TestSource == null ? null : string.Format("build_{0}_{1:yyyyMMdd_hhmm}", TestSource.Name, Created); }
        }

        public string LocalPath
        {
            get
            {
                return string.IsNullOrEmpty(Name)
                    ? null
                    : Path.ChangeExtension(Path.Combine(Constants.BUILD_ROOT, Name), ".zip");
            }
        }
    }
}