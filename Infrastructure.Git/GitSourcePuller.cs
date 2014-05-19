using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RunProcessAsTask;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TestLab.Infrastructure.Git
{
    public class GitSourcePuller : ISourcePuller
    {
        private static readonly Regex RxUrl = new Regex(@"http(s)?://.+/\w+\.git", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Implementation of ITestPuller

        public bool CanPull(string repoPathOrUrl)
        {
            return RxUrl.IsMatch(repoPathOrUrl);
        }

        public Task Pull(string repoPathOrUrl, string workDir)
        {
            var ss = repoPathOrUrl.Split(' ');
            string branch=null, url=ss[0];
            if (ss.Length > 1)
            {
                branch = ss[1];
            }

            ProcessStartInfo pi;

            var args = new List<string>();
            
            //git clone or git pull
            if (Directory.Exists(workDir))
            {
                args.Add("pull");
                args.Add(url);
                if (!string.IsNullOrEmpty(branch))
                {
                    args.Add(branch);
                }
                pi = new ProcessStartInfo("git", string.Join(" ", args)) { WorkingDirectory = workDir };
            }
            else
            {
                args.Add("clone");
                args.Add(url);
                if (!string.IsNullOrEmpty(branch))
                {
                    args.Add("-b");
                    args.Add(branch);
                }
                args.Add(workDir);
                pi = new ProcessStartInfo("git", string.Join(" ", args));
            }

            return ProcessEx.RunAsync(pi);
        }

        #endregion
    }
}