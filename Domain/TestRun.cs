using System;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestRun : TestJob
    {
        public virtual TestResult Result { get; set; }

        public virtual TestSession Session { get; set; }

        public virtual TestCase Case { get; set; }
    }
}