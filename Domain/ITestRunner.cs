using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestRunner
    {
        TestType Type { get; }

        Task<TestResult> Run(TestCase test, TestBuild build, TestConfig config);
    }
}