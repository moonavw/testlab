using TestLab.Domain.Models;

namespace TestLab.Domain.Services
{
    public interface ITestSourcePuller
    {
        void Pull(TestSource src);
    }
}