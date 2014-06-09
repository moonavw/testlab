using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Diagnostics;

namespace TestLab.Infrastructure.Zip
{
    public class ZipArchiver : IArchiver
    {
        #region Implementation of ITestArchiver

        public async Task Archive(string srcDir, string archiveFile)
        {
            var fi = new FileInfo(Path.ChangeExtension(archiveFile, ".zip"));
            if (!fi.Exists)
            {
                if (!fi.Directory.Exists)
                    fi.Directory.Create();
                await Task.Run(() =>
                {
                    Trace.TraceInformation("archiving {0} to {1}", srcDir, fi.FullName);
                    //zip files
                    using (var zip = new ZipFile(fi.FullName))
                    {
                        zip.AddDirectory(srcDir, null);
                        zip.Save();
                    }
                });
            }
        }

        public async Task Extract(string archiveFile, string destDir)
        {
            string filename = Path.ChangeExtension(archiveFile, ".zip");
            if (!Directory.Exists(destDir))
            {
                await Task.Run(() =>
                {
                    Trace.TraceInformation("extracting {0} to {1}", filename, destDir);
                    //unzip files
                    using (var zip = ZipFile.Read(filename))
                    {
                        zip.ExtractAll(destDir);
                    }
                });
            }
        }

        #endregion
    }
}