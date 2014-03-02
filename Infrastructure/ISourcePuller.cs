using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public interface ISourcePuller
    {
        bool CanPull(string repoPathOrUrl);

        Task Pull(string repoPathOrUrl, string workDir);
    }
}