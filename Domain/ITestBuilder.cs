using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public interface ITestBuilder
    {
        TestBuild Build(TestSource src);
    }
}