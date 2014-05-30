using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestResult : Entity
    {
        public int TestRunId { get; set; }

        public bool? PassOrFail { get; set; }

        public string Output { get; set; }

        public string Summary { get; set; }

        public string ErrorDetails { get; set; }

        public virtual TestRun Run { get; set; }
    }
}