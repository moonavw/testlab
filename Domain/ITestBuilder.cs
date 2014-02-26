using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestBuilder
    {
        Task Build(TestBuild build);
    }
}