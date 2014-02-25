using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestBuilder
    {
        TestSrcType Type { get; }

        Task Build(TestSrc src, TestBin bin);
    }

    //    public async Task Build(TestProject project)
    //    {
    //        string buildScript = project.BuildScript;
    //        string workDir = project.WorkDir;
    //        var pi = new ProcessStartInfo(buildScript) { WorkingDirectory = workDir };

    //        await ProcessEx.RunAsync(pi);
    //    }
}