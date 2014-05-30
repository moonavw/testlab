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

        public string LocalPath
        {
            get { return Path.Combine(Project.LocalPath, Constants.RESULT_DIR_NAME, ToString()); }
        }

        public string GetPathOnAgent(TestAgent agent)
        {
            return Path.Combine(Project.GetPathOnAgent(agent), Constants.RESULT_DIR_NAME, ToString());
        }

        public override string ToString()
        {
            return Name.Replace(" ", "");
        }
    }
}