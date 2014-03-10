using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NPatterns.ObjectRelational;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSession : Entity, IAuditable, IStartable
    {
        public TestSession()
        {
            Build = new TestBuild();
            Runs = new HashSet<TestRun>();
            Agent = new TestAgent();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public TestBuild Build { get; set; }

        public TestAgent Agent { get; set; }

        public string OutputDir
        {
            get { return Path.Combine(Constants.RESULT_ROOT, ToString()); }
        }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestRun> Runs { get; set; }

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

        #region Implementation of IAuditable

        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }

        #endregion

        #region Implementation of IStartable

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}_{1}", Project, Name.Replace(" ", ""));
        }


        public IEnumerable<TestAgent> GetAgents()
        {
            return from s in Agent.Server.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                   select new TestAgent
                   {
                       Server = s.Trim(),
                       Domain = Agent.Domain,
                       Password = Agent.Password,
                       UserName = Agent.UserName
                   };
        }
    }
}