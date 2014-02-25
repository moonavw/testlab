using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestRunner
    {
        TestBinType Type { get; }

        Task<TestResult> Run(TestCase test, TestBin bin, TestConfig config);
    }
}