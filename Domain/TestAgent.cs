using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestAgent
    {
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

        public string GetOutputDir(TestSession session)
        {
            string resultRoot = string.Format(Constants.AGENT_RESULT_ROOT_FORMAT, Server);
            return Path.Combine(resultRoot, session.ToString());
        }

        public string GetBuildDir(TestBuild build)
        {
            string buildRoot = string.Format(Constants.AGENT_BUILD_ROOT_FORMAT, Server);
            return Path.Combine(buildRoot, build.Name);
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", DomainUser, Server);
        }
    }
}