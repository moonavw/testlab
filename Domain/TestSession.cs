using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
            Bin = new TestBin();
            Results = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public TestConfig Config { get; set; }

        public TestBin Bin { get; set; }

        public int TestPlanId { get; set; }

        public virtual TestPlan Plan { get; set; }

        public virtual ICollection<TestResult> Results { get; set; }

        public int PassCount
        {
            get { return Results.Count(z => z.PassOrFail == true); }
        }

        public int FailCount
        {
            get { return Results.Count(z => z.PassOrFail == false); }
        }

        public int NoResultCount
        {
            get { return Results.Count(z => z.PassOrFail == null); }
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