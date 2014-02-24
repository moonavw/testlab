using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestReporter
    {
        bool CanReport(TestSession session);

        Task Report(TestSession session);
    }
}