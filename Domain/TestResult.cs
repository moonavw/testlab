using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestResult : ValueObject
    {
        public bool? PassOrFail { get; set; }

        public string Output { get; set; }

        public string Summary { get; set; }

        public string ErrorDetails { get; set; }
    }
}