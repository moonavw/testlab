using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSession : Entity, IAuditable, IArchivable
    {
        public TestSession()
        {
            Queues = new HashSet<TestQueue>();
            Config = new TestConfig();
        }

        public int Id { get; set; }

        public string Name
        {
            get { return string.Format("{1} {0:yyyyMMddhhmm}", Created, Plan.Name); }
        }

        public TestConfig Config { get; set; }

        public virtual TestBuild Build { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual TestPlan Plan { get; set; }

        public virtual ICollection<TestQueue> Queues { get; set; }

        public IReadOnlyCollection<TestRun> Runs
        {
            get { return Queues.SelectMany(z => z.Runs).ToList().AsReadOnly(); }
        }

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

        #region IAuditable Members

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        #region IArchivable Members

        public DateTime? Deleted { get; set; }
        public string DeletedBy { get; set; }

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

        public void SetAgents(IEnumerable<TestAgent> agents)
        {
            Queues = new HashSet<TestQueue>(agents.Select(z => new TestQueue { Agent = z }));

            var tests = Plan.Cases.Actives().ToList();
            int pageSize = (int)Math.Ceiling((double)tests.Count / Queues.Count);
            for (int i = 0; i < Queues.Count; i++)
            {
                Queues.ElementAt(i).Runs = new HashSet<TestRun>(tests.Skip(i * pageSize).Take(pageSize).Select(z => new TestRun { Case = z }));
            }
        }

        public override string ToString()
        {
            return Name.Replace(" ", "");
        }
    }
}