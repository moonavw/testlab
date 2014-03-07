using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Spi;

namespace TestLab.Application
{
    public class NinjectJobFactory : IJobFactory
    {
        #region IJobFactory Members

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            throw new NotImplementedException();
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
