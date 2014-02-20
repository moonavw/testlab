using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Domain.Models
{
    public class TestRun : Entity
    {
        public TestRun()
        {
            TestResults = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        public int TestBuildId { get; set; }

        public int TestPlanId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Completed { get; set; }

        public virtual TestPlan TestPlan { get; set; }

        public virtual TestBuild TestBuild { get; set; }

        public virtual ICollection<TestResult> TestResults { get; set; }

        public string Name
        {
            get { return TestPlan == null ? null : string.Format("{0}_{1:yyyyMMdd_hhmm}", TestPlan.Name, Created); }
        }
    }
}
