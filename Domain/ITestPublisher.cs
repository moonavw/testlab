using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestPublisher
    {
        bool CanPublish(TestProject project);

        Task Publish(TestProject project);
    }
}