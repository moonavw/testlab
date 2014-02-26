using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestArchiver
    {
        Task Archive(TestBuild build);

        Task Extract(TestBuild build, TestConfig config);
    }
}