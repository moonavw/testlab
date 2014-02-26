using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestPuller
    {
        bool CanPull(TestProject project);

        Task Pull(TestProject project);
    }
}