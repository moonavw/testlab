using System.ComponentModel.DataAnnotations;
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
    }
}