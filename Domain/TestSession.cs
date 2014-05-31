using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSession : Entity, IAuditable
    {
        public TestSession()
        {
            Runs = new HashSet<TestRun>();
            Queues = new HashSet<TestQueue>();
        }

        public int Id { get; set; }

        [RegularExpression("[a-zA-Z0-9 ]+")]
        [Required]
        public string Name { get; set; }

        public virtual TestBuild Build { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestQueue> Queues { get; set; }

        public virtual ICollection<TestRun> Runs { get; set; }

        #region info

        public int PassCount
        {
            get { return Runs.Count(z => z.Result.PassOrFail == true); }
        }

        public int FailCount
        {
            get { return Runs.Count(z => z.Result.PassOrFail == false); }
        }

        public int NoResultCount
        {
            get { return Runs.Count(z => z.Result.PassOrFail == null); }
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

        #endregion

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        public DateTime? Started
        {
            get { return Queues.OrderBy(z => z.Started).Select(z => z.Started).FirstOrDefault(); }
        }

        public DateTime? Completed
        {
            get { return Queues.OrderByDescending(z => z.Completed).Select(z => z.Completed).FirstOrDefault(); }
        }

        public string LocalPath
        {
            get { return Path.Combine(Project.LocalPath, Constants.RESULT_DIR_NAME, ToString()); }
        }

        public string GetPathOnAgent(TestAgent agent)
        {
            return Path.Combine(Project.GetPathOnAgent(agent), Constants.RESULT_DIR_NAME, ToString());
        }

        public void SetPlan(TestPlan plan)
        {
            Name = string.Format("{1} {0:yyyyMMddhhmm}", DateTime.Now, plan.Name);
            var tests = plan.Cases.Where(z => z.Published != null).ToList();
            Runs.Clear();
            Runs = new HashSet<TestRun>(tests.Select(z => new TestRun { Case = z }));
        }

        public void SetAgents(IEnumerable<TestAgent> agents)
        {
            Queues.Clear();
            Queues = new HashSet<TestQueue>(agents.Select(z => new TestQueue { Agent = z, Project = this.Project }));
        }

        public override string ToString()
        {
            return Name.Replace(" ", "");
        }
    }
}