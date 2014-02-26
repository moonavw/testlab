using System;
using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using TestLab.Domain;

namespace TestLab.Infrastructure.Zip
{
    public class ZipTestArchiver : ITestArchiver
    {
        #region Implementation of ITestArchiver

        public async Task Archive(TestBuild build)
        {
            string filename = Path.ChangeExtension(build.ArchivePath, ".zip");
            string srcDir = build.Project.BuildOutputDir;
            if (!File.Exists(filename))
            {
                await Task.Run(() =>
                {
                    //zip files
                    using (var zip = new ZipFile(filename))
                    {
                        zip.AddDirectory(srcDir, null);
                        zip.Save();
                    }
                    build.Archived = DateTime.Now;
                });
            }
        }

        public async Task Extract(TestBuild build, TestConfig config)
        {
            string filename = Path.ChangeExtension(build.ArchivePath, ".zip");
            string remotePath = Path.Combine(config.RemoteBuildRoot, build.Name);
            if (!Directory.Exists(remotePath))
            {
                await Task.Run(() =>
                {
                    //unzip files
                    using (var zip = ZipFile.Read(filename))
                    {
                        zip.ExtractAll(remotePath);
                    }
                });
            }
        }

        #endregion
    }
}