using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSession : Entity
    {
        public TestSession()
        {
            Config = new TestConfig();
            Runs = new HashSet<TestRun>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public TestConfig Config { get; set; }

        public int TestPlanId { get; set; }

        public virtual TestPlan Plan { get; set; }

        public virtual ICollection<TestRun> Runs { get; set; }

        public int PassCount
        {
            get { return Runs.Count(z => z.Result.Type == TestResultType.Pass); }
        }

        public int FailCount
        {
            get { return Runs.Count(z => z.Result.Type == TestResultType.Fail); }
        }

        public int NoResultCount
        {
            get { return Runs.Count(z => z.Result.Type == TestResultType.None); }
        }

        public TestResultType Type
        {
            get
            {
                if (NoResultCount > 0)
                    return TestResultType.None;
                if (FailCount > 0)
                    return TestResultType.Fail;
                return TestResultType.Pass;
            }
        }

        public string Summary
        {
            get
            {
                return string.Format("{0} pass, {1} fail, {2} no result",
                    PassCount,
                    FailCount,
                    NoResultCount);
            }
        }
    }
}