﻿using NPatterns.Messaging;
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
        private readonly IMessageBus _bus;
        private readonly ITestLabUnitOfWork _uow;
        private readonly IRepository<TestJob> _jobRepo;
        private readonly List<Task> _runningTasks;

        private CancellationTokenSource _source;
        private TestAgent _agent;

        public TestAgentService(ITestLabUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _jobRepo = uow.Repository<TestJob>();
            _bus = bus;
            _runningTasks = new List<Task>();
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

            _source = new CancellationTokenSource();
        }

        public void Start()
        {
            Trace.TraceInformation("start TestAgent: {0}", _agent.Name);
            _runningTasks.Clear();
            _runningTasks.Add(StartKeepAlive());

            _runningTasks.Add(Task.Run(() =>
            {
                while (!_source.Token.IsCancellationRequested)
                {
                    //get NotStarted jobs assigned to current agent
                    var jobs = (from e in _jobRepo.Query()
                                where e.Agent.Id == _agent.Id && (e.Started == null || e.Completed == null)
                                select e).ToList();

                    if (jobs.Count > 0)
                    {
                        Trace.TraceInformation("get {0} jobs for agent {1}", jobs.Count, _agent);
                        Task.WaitAll(
                            StartJobs(jobs.OfType<TestBuild>()),
                            StartJobs(jobs.OfType<TestQueue>())
                        );
                    }
                    else
                    {//just have a rest
                        Thread.Sleep(5000);
                    }
                }
                Trace.TraceInformation("stop TestAgent: {0}", _agent.Name);
            }, _source.Token));
        }

        public void Stop()
        {
            _source.Cancel();
            Task.WaitAll(_runningTasks.ToArray());
            _runningTasks.Clear();
        }

        private Task StartKeepAlive()
        {
            return Task.Run(async () =>
            {
                while (!_source.Token.IsCancellationRequested)
                {
                    _agent.LastTalked = DateTime.Now;
                    await _uow.CommitAsync();
                    Thread.Sleep(Constants.AGENT_KEEPALIVE * 1000);
                }
            }, _source.Token);
        }

        private Task StartJobs<T>(IEnumerable<T> jobs) where T : TestJob
        {
            return Task.Run(() =>
            {
                foreach (var job in jobs)
                {
                    if (_source.Token.IsCancellationRequested)
                        break;

                    _bus.Publish(job);
                }
            }, _source.Token);
        }
    }
}
