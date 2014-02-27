using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestArchiver
    {
        Task Archive(TestBuild build);

        Task Extract(TestBuild build, TestSession session);
    }
}