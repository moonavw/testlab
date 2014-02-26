using System.ComponentModel.DataAnnotations;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestConfig : ValueObject
    {
        [Required]
        public string RdpServer { get; set; }

        [Required]
        public string RdpDomain { get; set; }

        [Required]
        public string RdpUserName { get; set; }

        [Required]
        public string RdpPassword { get; set; }

        public string RemoteBuildRoot
        {
            get { return string.Format(Constants.REMOTE_BUILD_ROOT_FORMAT, RdpServer); }
        }

        public string RemoteResultRoot
        {
            get { return string.Format(Constants.REMOTE_RESULT_ROOT_FORMAT, RdpServer); }
        }
    }
}