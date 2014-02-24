using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestBuilder
    {
        bool CanBuild(TestProject project);

        Task Build(TestProject project);
    }
}