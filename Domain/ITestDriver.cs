using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestDriver
    {
        string Name { get; }

        Task<IEnumerable<TestCase>> Publish(TestProject project);

        Task<TestResult> Run(TestRun run);

        TestRunTask CreateTask(TestRun run);

        Task<TestResult> ParseResult(TestRunTask task);
    }
}