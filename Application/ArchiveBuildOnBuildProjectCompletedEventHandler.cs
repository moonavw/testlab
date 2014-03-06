using NPatterns;
using System;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class ArchiveBuildOnBuildProjectCompletedEventHandler : IHandler<BuildProjectCompletedEvent>
    {
        private readonly IArchiver _archiver;

        public ArchiveBuildOnBuildProjectCompletedEventHandler(
            IArchiver archiver)
        {
            _archiver = archiver;
        }

        #region IHandler<BuildProjectCompletedEvent> Members

        public void Handle(BuildProjectCompletedEvent message)
        {
            Task.WaitAll(HandleAsync(message));
        }

        public async Task HandleAsync(BuildProjectCompletedEvent message)
        {
            var project = message.Project;
            var build = project.Build;

            //archive
            await _archiver.Archive(project.BuildOutputDir, build.Location);
        }

        #endregion
    }
}
