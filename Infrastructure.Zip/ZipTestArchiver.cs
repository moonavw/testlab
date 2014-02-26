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
            var fi = new FileInfo(Path.ChangeExtension(build.ArchivePath, ".zip"));
            string srcDir = build.Project.BuildOutputDir;
            if (!fi.Exists)
            {
                if (!fi.Directory.Exists)
                    fi.Directory.Create();
                await Task.Run(() =>
                {
                    //zip files
                    using (var zip = new ZipFile(fi.FullName))
                    {
                        zip.AddDirectory(srcDir, null);
                        zip.Save();
                    }
                });
                build.Archived = DateTime.Now;
            }
            else
            {
                build.Archived = fi.CreationTime;
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