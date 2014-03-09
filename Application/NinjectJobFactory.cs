using System;
using System.Diagnostics;
using Ninject;
using Quartz;
using Quartz.Spi;

namespace TestLab.Application
{
    public class NinjectJobFactory : IJobFactory
    {
        private readonly Func<IKernel> _kernelFactory;

        public NinjectJobFactory(Func<IKernel> kernelFactory)
        {
            _kernelFactory = kernelFactory;
        }

        #region IJobFactory Members

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJobDetail jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;
            Debug.WriteLine("Producing instance of Job '{0}', class={1}", jobDetail.Key, jobType.FullName);
            return (IJob) _kernelFactory().Get(jobType);
        }

        public void ReturnJob(IJob job)
        {
            _kernelFactory().Release(job);
        }

        #endregion
    }
}
