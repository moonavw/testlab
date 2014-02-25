using System.ComponentModel.DataAnnotations;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestRepo : ValueObject
    {
        [Required]
        public string PathOrUrl { get; set; }

        public TestRepoType Type { get; set; }
    }
}