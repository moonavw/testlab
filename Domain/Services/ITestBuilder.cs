using TestLab.Domain.Models;

namespace TestLab.Domain.Services
{
    public interface ITestBuilder
    {
        TestBuild Build(TestSource src);
    }
}