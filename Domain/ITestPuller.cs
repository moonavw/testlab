using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestPuller
    {
        TestRepoType Type { get; }

        Task Pull(TestSrc src, TestRepo repo);
    }
}