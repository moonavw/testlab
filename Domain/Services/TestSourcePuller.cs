using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Domain.Models;
using System.IO;
using System.Diagnostics;

namespace TestLab.Domain.Services
{
    public interface ITestSourcePuller
    {
        void Pull(TestSource src);
    }

    public class GitTestSourcePuller : ITestSourcePuller
    {
        public void Pull(TestSource src)
        {
            //git clone or git pull
            if (!Directory.Exists(src.LocalPath))
            {//git clone for the first time
                Process
                    .Start("git", string.Format("clone {0} {1}", src.SourcePath, src.LocalPath))
                    .WaitForExit();
            }
            else
            {//git pull
                var pi = new ProcessStartInfo("git", "pull")
                {
                    WorkingDirectory = src.LocalPath
                };
                Process.Start(pi)
                    .WaitForExit();
            }
        }
    }
}
