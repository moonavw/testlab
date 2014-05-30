using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestDriver
    {
        string Name { get; }

        Task<IEnumerable<TestCase>> Publish(TestBuild build);

        Task<TestResult> Run(TestRun run, TestAgent agent);
    }
}