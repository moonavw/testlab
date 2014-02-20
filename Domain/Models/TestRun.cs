using System;
using System.Collections.Generic;

namespace TestLab.Domain.Models
{
    public class TestRun : Entity
    {
        public int Id { get; set; }

        public int TestBuildId { get; set; }

        public int TestPlanId { get; set; }

        public DateTime Created { get; set; }

        public virtual TestPlan TestPlan { get; set; }

        public virtual TestBuild TestBuild { get; set; }

        public virtual TestReport TestReport { get; set; }

        public string Name
        {
            get { return TestPlan == null ? null : string.Format("run_{0}_{1:yyyyMMdd_hhmm}", TestPlan.Name, Created); }
        }
    }
}