using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestRunner
    {
        bool CanRun(TestSession session);

        Task Run(TestSession session);
    }
}