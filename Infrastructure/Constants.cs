using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Infrastructure
{
    public static class Constants
    {
        public static readonly string SRC_ROOT = @"d:\test\src\";
        public static readonly string BUILD_ROOT = @"d:\test\builds\";
        public static readonly string RUN_ROOT = @"\\esfvmvi-2205\d$\test\runs\";//@"d:\test\runs\";
        public static readonly string RESULT_ROOT = @"d:\test\results\";

        public static readonly string RDP_TOOL = @"d:\MMtool\rdp\rdpex.exe";
        public static readonly string RDP_SERVER = "esfvmvi-2205";
        public static readonly string RDP_DOMAIN = "sea";
        public static readonly string RDP_USER = "v-twang";
        public static readonly string RDP_PWD = "tw#201401";
        public static readonly string RDP_START = @"d:\test\runs\start.cmd";
    }
}
