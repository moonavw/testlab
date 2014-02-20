using System.Collections.Generic;
using System.IO;

namespace TestLab.Domain.Models
{
    public class TestCase : Entity
    {
        public TestCase()
        {
            TestPlans = new HashSet<TestPlan>();
            TestResults = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public int TestSourceId { get; set; }

        public virtual ICollection<TestPlan> TestPlans { get; set; }

        public virtual ICollection<TestResult> TestResults { get; set; }

        public virtual TestSource TestSource { get; set; }

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