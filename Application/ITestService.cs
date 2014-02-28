using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Application
{
    public interface ITestService
    {
        Task Build(TestProject project);

        Task Run(TestSession session);

        Task Run(TestRun run);
    }
}