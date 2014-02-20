using TestLab.Infrastructure;

namespace TestLab.Domain.Services
{
    public interface ITestBuilder
    {
        TestBuild Build(TestSource src);
    }
}