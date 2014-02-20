using System.Collections.Generic;

namespace TestLab.Domain.Models
{
    public class TestPlan : Entity
    {
        public TestPlan()
        {
            TestCases = new HashSet<TestCase>();
            TestRuns = new HashSet<TestRun>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<TestCase> TestCases { get; set; }

        public virtual ICollection<TestRun> TestRuns { get; set; }
    }
}