using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public interface ITestSourcePuller
    {
        void Pull(TestSource src);
    }
}