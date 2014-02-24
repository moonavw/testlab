using System;
using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Infrastructure.Ruby
{
    public class RubyTestBuilder : ITestBuilder
    {
        private readonly IUnitOfWork _uow;

        public RubyTestBuilder(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool CanBuild(TestProject project)
        {
            return project.Build.Type == TestBuildType.Ruby;
        }

        public async Task Build(TestProject project)
        {
            //TODO: rake or just copy

            project.Build.LocalPath = project.LocalPath;
            project.Build.Built = DateTime.Now;
            project.Build.Name = string.Format("build_{0}_{1:yyyyMMdd_hhmm}", project.Name, project.Build.Built);

            await _uow.CommitAsync();
        }
    }
}
