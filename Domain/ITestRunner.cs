using System.Threading.Tasks;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public interface ITestRunner
    {
        TestRun Run(TestPlan plan, TestBuild build);
        Task<TestRun> RunAsync(TestPlan plan, TestBuild build);
    }
}