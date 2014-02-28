using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestBuilder
    {
        Task<TestBuild> Build(TestProject project);
    }
}