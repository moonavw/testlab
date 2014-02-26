using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Infrastructure.Cucumber
{
    public class CucumberTestPublisher : ITestPublisher
    {
        private static readonly Regex RxTag = new Regex(@"@Name_(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Implementation of ITestPublisher

        public TestType Type
        {
            get { return TestType.Cucumber; }
        }

        public async Task<IEnumerable<TestCase>> Publish(TestBuild build)
        {
            string srcPath = build.Project.WorkDir;
            //looking for test files, publish them to db
            var dir = new DirectoryInfo(srcPath);
            var files = dir.GetFiles("*.feature", SearchOption.AllDirectories);
            var tests = new List<TestCase>();
            foreach (var f in files)
            {
                string text = await f.OpenText().ReadToEndAsync();
                var matches = RxTag.Matches(text);
                tests.AddRange(from Match m in matches
                               select new TestCase
                               {
                                   Published = DateTime.Now,
                                   Name = m.Groups[1].Value,
                                   FullName = f.FullName.Remove(0, srcPath.Length)
                               });
            }

            return tests;
        }

        #endregion
    }
}