using NPatterns.Messaging;
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
        private readonly IMessageBus _bus;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestJob> _jobRepo;

        private TestAgent _agent;

        public TestAgentService(IUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _jobRepo = uow.Repository<TestJob>();
            _bus = bus;
        }

        public void Initialize(string agentName)
        {
            //find current agent in repository
            var agentRepo = _uow.Repository<TestAgent>();
            _agent = agentRepo.Query().FirstOrDefault(z => z.Name == agentName);

            Debug.WriteLine("{1} TestAgent: {0}", agentName, _agent == null ? "Create" : "Update");

            //create or update agent in repository
            if (_agent == null)
            {
                _agent = new TestAgent { Name = agentName };
                agentRepo.Add(_agent);
                _uow.Commit();
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("start TestAgent: {0}", _agent.Name);

            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //KeepAlive
                    if (!_agent.IsOnline)
                    {
                        _agent.LastTalked = DateTime.Now;
                        await _uow.CommitAsync();
                    }

                    //get NotStarted jobs assigned to current agent
                    var jobs = (from e in _jobRepo.Query()
                                where e.Agent.Id == _agent.Id && (e.Started == null || e.Completed == null)
                                select e).ToList();

                    if (jobs.Count > 0)
                    {
                        Trace.TraceInformation("get {0} jobs for agent {1}", jobs.Count, _agent);
                        Task.WaitAll(
                            StartJobs(jobs.OfType<TestBuild>(), cancellationToken),
                            StartJobs(jobs.OfType<TestQueue>(), cancellationToken)
                        );
                    }
                    else
                    {//just have a rest
                        Thread.Sleep(5000);
                    }
                }
                Trace.TraceInformation("stop TestAgent: {0}", _agent.Name);
            }, cancellationToken);
        }

        private Task StartJobs<T>(IEnumerable<T> jobs, CancellationToken cancellationToken) where T : TestJob
        {
            return Task.Run(() =>
            {
                foreach (var job in jobs)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    _bus.Publish(job);
                }
            }, cancellationToken);
        }
    }
}
