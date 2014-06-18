using NPatterns.Messaging;
using NPatterns.ObjectRelational;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestAgentService
    {
        public event EventHandler<Exception> OnError;

        private readonly Func<ITestLabUnitOfWork> _uowFactory;
        private readonly IMessageBus _bus;

        private Task _worker;
        private CancellationTokenSource _source;
        private TestAgent _agent;
        private ITestLabUnitOfWork _uow;

        public TestAgentService(Func<ITestLabUnitOfWork> uowFactory, IMessageBus bus)
        {
            _uowFactory = uowFactory;
            _bus = bus;
        }

        protected ITestLabUnitOfWork Uow
        {
            get
            {
                if (_uow == null)
                    _uow = _uowFactory();
                return _uow;
            }
        }

        private void Initialize()
        {
            string agentName = Environment.MachineName;
            //find current agent in repository
            var agentRepo = Uow.Repository<TestAgent>();
            _agent = agentRepo.Query().FirstOrDefault(z => z.Name == agentName);

            Debug.WriteLine("{1} TestAgent: {0}", agentName, _agent == null ? "Create" : "Update");

            //create or update agent in repository
            if (_agent == null)
            {
                _agent = new TestAgent { Name = agentName };
                agentRepo.Add(_agent);
                Uow.Commit();
            }

            _source = new CancellationTokenSource();
            Trace.TraceInformation("start TestAgent: {0}", _agent.Name);
        }

        public void Start()
        {
            Initialize();
            _worker = Task.Run(() =>
            {
                while (!_source.Token.IsCancellationRequested)
                {
                    try
                    {
                        KeepAlive();
                        if (StartJobs() == 0)
                        {//just have a rest
                            Thread.Sleep(Constants.POLLING_INTERVAL * 1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex);
                    }
                    finally
                    {
                        _uow.Dispose();
                        _uow = null;
                    }
                }
            }, _source.Token);
        }

        public void Stop()
        {
            Trace.TraceInformation("stop TestAgent: {0}", _agent.Name);
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
                Trace.TraceWarning("Test Agent: {0} has to be stopped since got error", _agent.Name);
                Stop();
            }
        }

        private void KeepAlive()
        {
            _agent.Active();
            _agent.LastTalked = DateTime.Now;
            Uow.Commit();
        }

        private int StartJobs()
        {
            //get NotStarted jobs assigned to current agent
            var jobs = (from e in Uow.Repository<TestJob>().Query()
                        where e.Agent.Id == _agent.Id && (e.Started == null || e.Completed == null)
                        select e).ToList();

            if (jobs.Count > 0)
            {
                Trace.TraceInformation("get {0} jobs for agent {1}", jobs.Count, _agent);
                StartJobs(jobs.OfType<TestBuild>());
                StartJobs(jobs.OfType<TestQueue>());

                Trace.TraceInformation("complete {0} jobs by agent {1}", jobs.Count, _agent);
            }

            return jobs.Count;
        }

        private void StartJobs<T>(IEnumerable<T> jobs) where T : TestJob
        {
            foreach (var job in jobs)
            {
                if (_source.Token.IsCancellationRequested)
                    break;
                try
                {
                    _bus.Publish(job);
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }
    }
}
