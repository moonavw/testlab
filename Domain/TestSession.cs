using System;
using System.ComponentModel.DataAnnotations;
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
            Build = new TestBuild();
            Runs = new HashSet<TestRun>();
        }

        public int Id { get; set; }

        public string Name
        {
            get { return string.Format("{0}_session_{1:yyyyMMdd_hhmm}", Plan.Project.Name, Created); }
        }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

        public TestBuild Build { get; set; }

        [Required]
        public string Server { get; set; }

        [Required]
        public string Domain { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string EncryptedPassword
        {
            get { return PasswordProtector.Encrypt(Password, Constants.EncryptionKey); }
            set { Password = PasswordProtector.Decrypt(value, Constants.EncryptionKey); }
        }

        public string DomainUser
        {
            get { return string.Format(@"{0}\{1}", Domain, UserName); }
        }

        public string RemoteBuildRoot
        {
            get { return string.Format(Constants.REMOTE_BUILD_ROOT_FORMAT, Server); }
        }

        public string RemoteResultRoot
        {
            get { return string.Format(Constants.REMOTE_RESULT_ROOT_FORMAT, Server); }
        }

        public int TestPlanId { get; set; }

        public virtual TestPlan Plan { get; set; }

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
    }
}