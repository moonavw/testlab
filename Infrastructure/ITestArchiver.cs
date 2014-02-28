using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public interface IArchiver
    {
        Task Archive(string srcDir, string archiveFile);

        Task Extract(string archiveFile, string destDir);
    }
}