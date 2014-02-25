using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBin : ValueObject
    {
        public TestBinType Type { get; set; }

        public string Location { get; set; }

        public DateTime? Built { get; set; }
    }
}