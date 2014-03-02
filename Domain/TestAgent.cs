using System.ComponentModel.DataAnnotations;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestAgent
    {
        public TestAgent()
        {
        }

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

        public string BuildRoot
        {
            get { return string.Format(Constants.AGENT_BUILD_ROOT_FORMAT, Server); }
        }

        public string ResultRoot
        {
            get { return string.Format(Constants.AGENT_RESULT_ROOT_FORMAT, Server); }
        }
    }
}