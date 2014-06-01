using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : TestJob
    {
        public virtual TestProject Project { get; set; }

        public string Name
        {
            get { return string.Format("{0:yyyyMMddhhmm}", Created); }
        }

        public string LocalPath
        {
            get { return Path.Combine(Project.LocalPath, Constants.BUILD_DIR_NAME, Name); }
        }

        public string SharedPath
        {
            get
            {
                if (Agent == null) return null;
                return Path.Combine(Project.GetPathOnAgent(Agent), Constants.BUILD_DIR_NAME, Name);
            }
        }
    }
}