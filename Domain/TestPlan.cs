using System.Collections.Generic;
using TestLab.Infrastructure;

namespace TestLab.Domain
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