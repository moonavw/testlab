using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Application
{
    public interface ITestService
    {
        Task Build(TestBuild build);

        Task Run(TestSession session);
    }
}