using Ninject;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestAgentService
    {
        public event EventHandler<Exception> OnError;

        private readonly IKernel _kernel;
        private readonly string _agentName;

        private Task _worker;
        private CancellationTokenSource _source;

        public TestAgentService(IKernel kernel)
        {
            _kernel = kernel;
            _agentName = Environment.MachineName;
        }

        public void Start()
        {
            Trace.TraceInformation("start TestAgent: {0}", _agentName);
            _source = new CancellationTokenSource();
            _worker = Task.Run(() =>
            {
                while (!_source.Token.IsCancellationRequested)
                {
                    try
                    {
                        var proxy = _kernel.Get<TestAgentProxy>();
                        proxy.Initialize(_agentName);
                        if (proxy.StartJobs() == 0)
                        {//just have a rest
                            Thread.Sleep(Constants.POLLING_INTERVAL * 1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex);
                    }
                }
            }, _source.Token);
        }

        public void Stop()
        {
            Trace.TraceInformation("stop TestAgent: {0}", _agentName);
            _source.Cancel();
            Task.WaitAll(_worker);
        }

        private void HandleError(Exception ex)
        {
            Trace.TraceError(ex.ToString());
            if (OnError != null)
            {
                OnError(this, ex);
            }
            else
            {
                Trace.TraceWarning("Test Agent: {0} has to be stopped since got error", _agentName);
                Stop();
            }
        }
    }
}
