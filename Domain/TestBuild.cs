using System;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : ValueObject
    {
        public string Name { get; set; }

        public TestBuildType Type { get; set; }

        public string LocalPath { get; set; }

        public DateTime? Built { get; set; }

        public DateTime? Published { get; set; }

        public string PublishPath
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