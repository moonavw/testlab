using System;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : ValueObject
    {
        public string Name { get; set; }

        public string ArchivePath
        {
            get { return Path.Combine(Constants.BUILD_ROOT, Name); }
        }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public DateTime? Archived { get; set; }
    }
}