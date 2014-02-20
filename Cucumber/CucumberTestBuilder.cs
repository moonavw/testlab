using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ionic.Zip;
using MoreLinq;
using TestLab.Domain;
using TestLab.Domain.Models;
using TestLab.Domain.Services;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestBuilder : ITestBuilder
    {
        private static readonly Regex RxTag = new Regex(@"@Name_(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IUnitOfWork _uow;

        public CucumberTestBuilder(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public TestBuild Build(TestSource src)
        {
            //looking for test files, publish them to db
            var dir = new DirectoryInfo(src.LocalPath);
            var files = dir.GetFiles("*.feature", SearchOption.AllDirectories);

            var q = (from f in files
                     let text = f.OpenText().ReadToEnd()
                     let matches = RxTag.Matches(text)
                     from Match m in matches
                     select new TestCase
                     {
                         Name = m.Groups[1].Value,
                         FullName = f.FullName.Remove(0, src.LocalPath.Length)
                     }).ToList();

            var toDel = src.TestCases.ExceptBy(q, z => z.FullName + "@" + z.Name);
            var toAdd = q.ExceptBy(src.TestCases, z => z.FullName + "@" + z.Name);

            toDel.ForEach(z => src.TestCases.Remove(z));
            toAdd.ForEach(z => src.TestCases.Add(z));

            //create build
            var build = new TestBuild { Created = DateTime.Now };
            src.TestBuilds.Add(build);

            _uow.Commit();

            //zip files to build folder
            //TODO: zip files async
            using (var zip = new ZipFile(build.LocalPath))
            {
                zip.AddDirectory(src.LocalPath, null);
                zip.Save();
            }

            return build;
        }
    }
}