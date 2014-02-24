using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ionic.Zip;
using MoreLinq;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestPublisher : ITestPublisher
    {
        private static readonly Regex RxTag = new Regex(@"@Name_(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _uow;

        public CucumberTestPublisher(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool CanPublish(TestProject project)
        {
            return project.Type == TestProjectType.Cucumber;
        }

        public async Task Publish(TestProject project)
        {
            //looking for test files, publish them to db
            var dir = new DirectoryInfo(project.Build.LocalPath);
            var files = dir.GetFiles("*.feature", SearchOption.AllDirectories);

            var q = (from f in files
                     let text = f.OpenText().ReadToEnd()
                     let matches = RxTag.Matches(text)
                     from Match m in matches
                     select new TestCase
                     {
                         Name = m.Groups[1].Value,
                         FullName = f.FullName.Remove(0, project.Build.LocalPath.Length)
                     }).ToList();

            var toDel = project.Cases.ExceptBy(q, z => z.FullName + "@" + z.Name);
            var toAdd = q.ExceptBy(project.Cases, z => z.FullName + "@" + z.Name);

            toDel.ForEach(z => project.Cases.Remove(z));
            toAdd.ForEach(z => project.Cases.Add(z));

            //zip files to build publish folder
            //TODO: zip files async
            using (var zip = new ZipFile(project.Build.PublishPath))
            {
                zip.AddDirectory(project.Build.LocalPath, null);
                zip.Save();
            }

            project.Build.Published = DateTime.Now;
            await _uow.CommitAsync();
        }
    }
}