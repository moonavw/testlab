using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public interface IRepoPuller
    {
        bool CanPull(string repoPathOrUrl);

        Task Pull(string repoPathOrUrl, string workDir);
    }
}