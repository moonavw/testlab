using System.Threading.Tasks;
using TestLab.Domain.Models;

namespace TestLab.Domain.Services
{
    public interface ITestRunner
    {
        TestRun Run(TestPlan plan, TestBuild build);
        Task<TestRun> RunAsync(TestPlan plan, TestBuild build);
    }
}