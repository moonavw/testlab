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
            Results = new HashSet<TestResult>();
        }

        public int Id { get; set; }

        public string Name
        {
            get { return string.Format("{0}_session_{1:yyyyMMdd_hhmm}", Plan.Project.Name, Started); }
        }

        public DateTime? Started { get; set; }

        public DateTime? Completed { get; set; }

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
            get { return Encrypter.Encrypt(Password, Constants.EncryptionKey); }
            set { Password = Encrypter.Decrypt(value, Constants.EncryptionKey); }
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