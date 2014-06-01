﻿using NPatterns.Messaging;
using System;
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
            Debug.WriteLine("init Test Agent: {0}", agentName);
            //find current agent in repository
            var agentRepo = _uow.Repository<TestAgent>();
            _agent = agentRepo.Query().FirstOrDefault(z => z.Name == agentName);

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
            Trace.TraceInformation("start Test Agent: {0}", _agent.Name);

            StartJobs<TestBuild>(cancellationToken);
            StartJobs<TestQueue>(cancellationToken);

            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //KeepAlive
                    _agent.LastTalked = DateTime.Now;
                    await _uow.CommitAsync();
                    Thread.Sleep(Constants.AGENT_KEEPALIVE * 1000);
                }
                Trace.TraceInformation("stop Test Agent: {0}", _agent.Name);
            }, cancellationToken);
        }

        private Task StartJobs<T>(CancellationToken cancellationToken) where T : TestJob
        {
            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //get NotStarted jobs assigned to current agent
                    var jobs = (from e in _jobRepo.Query().OfType<T>()
                                where e.Agent.Id == _agent.Id && (e.Started == null || e.Completed == null)
                                select e).ToList();

                    if (jobs.Count > 0)
                    {
                        Trace.TraceInformation("get {0} {2} job for agent {1}", jobs.Count, _agent, typeof(T).Name);
                        foreach (var job in jobs)
                        {
                            _bus.Publish(job);
                        }
                    }
                    else
                    {//just has a rest
                        Thread.Sleep(Constants.AGENT_KEEPALIVE * 1000);
                    }
                }

            }, cancellationToken);
        }
    }
}
