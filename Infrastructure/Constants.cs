using System.IO;
using System.Configuration;

namespace TestLab.Infrastructure
{
    public static class Constants
    {
        public static readonly string APP_NAME = "TestLab";
        public static readonly string BUILD_DIR_NAME = "builds";
        public static readonly string SRC_DIR_NAME = "src";
        public static readonly string RESULT_DIR_NAME = "results";

        //format: d:\testlab\
        public static readonly string LOCAL_ROOT = Path.Combine(@"d:\", APP_NAME);

        //format: \\{machine}\testlab\
        public static readonly string AGENT_ROOT_FORMAT = Path.Combine(@"\\{0}\", APP_NAME);

        public static readonly int AGENT_KEEPALIVE = 10 * 60;//sec

        public static readonly int POLLING_INTERVAL = int.Parse(ConfigurationManager.AppSettings["PollingInterval"] ?? "30");//sec
    }
}