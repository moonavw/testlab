using TestLab.Infrastructure;

namespace TestLab.Domain.Services
{
    public interface ITestSourcePuller
    {
        void Pull(TestSource src);
    }
}