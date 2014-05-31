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
        protected readonly IUnitOfWork Uow;

        protected TestJobHandlerBase(IUnitOfWork uow)
        {
            Uow = uow;
        }

        protected abstract Task Run(T job);

        #region IHandler<T> Members

        public void Handle(T message)
        {
            throw new NotImplementedException();
        }

        public async Task HandleAsync(T message)
        {
            try
            {
                var job = message;

                Debug.WriteLine("Started {0}-{1} on agent {2}", typeof(T).Name, job.Id, job.Agent);

                job.Started = DateTime.Now;
                await Uow.CommitAsync();

                await Run(job);

                job.Completed = DateTime.Now;
                await Uow.CommitAsync();

                Debug.WriteLine("Completed {0}-{1} on agent {2}", typeof(T).Name, job.Id, job.Agent);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion
    }
}
