using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestService
    {
        Task Build(TestProject project);

        Task Run(TestSession session);
    }
}