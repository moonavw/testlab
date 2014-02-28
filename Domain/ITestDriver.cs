using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestDriver
    {
        string Name { get; }

        Task<IEnumerable<TestCase>> Publish(TestProject project);

        Task<TestResult> Run(TestCase test, TestBuild build, TestSession session);
    }
}