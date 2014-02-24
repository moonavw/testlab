using System;
using System.Collections.Generic;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestCase : Entity
    {
        public TestCase()
        {
            Plans = new HashSet<TestPlan>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public DateTime? Published { get; set; }

        public int TestProjectId { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public string FileName
        {
            get { return Path.GetFileName(FullName); }
        }

        public string DirectoryName
        {
            get { return Path.GetDirectoryName(FullName); }
        }
    }
}