using NPatterns;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public abstract class TestJobHandlerBase<T> : IHandler<T> where T : TestJob
    {
        protected readonly ITestLabUnitOfWork Uow;

        protected TestJobHandlerBase(ITestLabUnitOfWork uow)
        {
            Uow = uow;
        }

        protected abstract Task Run(T job);

        #region IHandler<T> Members

        public void Handle(T message)
        {
            HandleAsync(message).Wait();
        }

        public async Task HandleAsync(T message)
        {
            var job = message;
            Uow.MarkUnchanged(job);

            Debug.WriteLine("Started job-{0} {2} on agent {1}", job.Id, job.Agent, typeof(T).BaseType.Name);

            job.Started = DateTime.Now;
            await Uow.CommitAsync();

            await Run(job);

            job.Completed = DateTime.Now;
            await Uow.CommitAsync();

            Debug.WriteLine("Completed job-{0} {2} on agent {1}", job.Id, job.Agent, typeof(T).BaseType.Name);
        }

        #endregion
    }
}
