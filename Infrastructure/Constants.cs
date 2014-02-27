namespace TestLab.Infrastructure
{
    public static class Constants
    {
        public static readonly string DATETIME_FORMAT = "yyyyMMdd_hhmm";

        public static readonly string RDP_CLIENT = @"d:\MMtool\rdp\rdp.exe";

        public static readonly string BUILD_ROOT = @"d:\testlab\builds\";
        public static readonly string PROJ_ROOT = @"d:\testlab\projects\";
        public static readonly string RESULT_ROOT = @"d:\testlab\results";

        public static readonly string REMOTE_BUILD_ROOT_FORMAT = @"\\{0}\d$\testlab\builds";
        public static readonly string REMOTE_RESULT_ROOT_FORMAT = @"\\{0}\d$\testlab\results";
    }
}