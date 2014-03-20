using System.IO;

namespace TestLab.Infrastructure
{
    public static class Constants
    {
        public static readonly string AppName = "TestLab";

        public static readonly string EncryptionKey = AppName.ToLowerInvariant();

        public static readonly string BUILD_ROOT = Path.Combine(@"d:\", AppName, "builds");
        public static readonly string PROJ_ROOT = Path.Combine(@"d:\", AppName, "projects");
        public static readonly string RESULT_ROOT = Path.Combine(@"d:\", AppName, "results");

        public static readonly string AGENT_BUILD_ROOT_FORMAT = Path.Combine(@"\\{0}\d$\", AppName, "builds");
        public static readonly string AGENT_RESULT_ROOT_FORMAT = Path.Combine(@"\\{0}\d$\", AppName, "results");
    }
}