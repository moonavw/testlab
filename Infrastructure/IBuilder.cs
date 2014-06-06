using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public interface IBuilder
    {
        Task Build(string buildScript, string workDir);
    }
}