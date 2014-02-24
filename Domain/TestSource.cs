using System;
using System.ComponentModel.DataAnnotations;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSource : ValueObject
    {
        [Required]
        public string PathOrUrl { get; set; }

        public TestSourceType Type { get; set; }

        public DateTime? Pulled { get; set; }
    }
}