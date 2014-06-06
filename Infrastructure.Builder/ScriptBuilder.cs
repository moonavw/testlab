using RunProcessAsTask;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestLab.Infrastructure.Builder
{
    public class ScriptBuilder : IBuilder
    {
        #region IBuilder Members

        public async Task Build(string buildScript, string workDir)
        {
            if (!string.IsNullOrEmpty(buildScript))
            {
                var pi = new ProcessStartInfo(buildScript) { WorkingDirectory = workDir };
                await ProcessEx.RunAsync(pi);
            }
        }

        #endregion
    }
}