using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using TestLab.Domain;

namespace TestLab.Infrastructure.Zip
{
    public class ZipTestDeployer : ITestDeployer
    {
        #region Implementation of ITestDeployer

        public async Task Deploy(TestBin srcBin, TestBin destBin)
        {
            string zipFilename = Path.ChangeExtension(destBin.Location, ".zip");

            await Task.Run(() =>
            {
                if (!File.Exists(zipFilename))
                {
                    //zip files
                    using (var zip = new ZipFile(zipFilename))
                    {
                        zip.AddDirectory(srcBin.Location, null);
                        zip.Save();
                    }

                }
                if (!Directory.Exists(destBin.Location))
                {
                    //unzip files
                    using (var zip = ZipFile.Read(zipFilename))
                    {
                        zip.ExtractAll(destBin.Location);
                    }

                }
            });
        }

        #endregion
    }
}
