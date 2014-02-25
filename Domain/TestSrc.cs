using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestSrc : ValueObject
    {
        public TestSrcType Type { get; set; }

        public string Location { get; set; }

        public DateTime? Pulled { get; set; }
    }
}