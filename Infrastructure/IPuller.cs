using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public interface IPuller
    {
        bool CanPull(string repoPathOrUrl);

        Task Pull(string repoPathOrUrl, string workDir);
    }
}