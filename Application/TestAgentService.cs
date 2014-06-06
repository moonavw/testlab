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
        private Task _runningTask;

        private CancellationTokenSource _source;
        private TestAgent _agent;

        public TestAgentService(ITestLabUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _jobRepo = uow.Repository<TestJob>();
            _bus = bus;
        }

        private void Initialize()
        {
            string agentName = Environment.MachineName;
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
            Trace.TraceInformation("start TestAgent: {0}", _agent.Name);
        }

        public void Start()
        {
            Initialize();
            _runningTask = Task.Run(() =>
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
                        OnError(ex);
                    }
                }
            }, _source.Token);
        }

        public void Stop()
        {
            Trace.TraceInformation("stop TestAgent: {0}", _agent.Name);
            _source.Cancel();
            Task.WaitAll(_runningTask);
        }

        private void OnError(Exception ex)
        {
            Trace.TraceError(ex.ToString());
            _source.Cancel();
            Trace.TraceWarning("Test Agent: {0} has to be stopped since got error", _agent.Name);
        }

        private void KeepAlive()
        {
            _agent.Active();
            _agent.LastTalked = DateTime.Now;
            _uow.Commit();
        }

        private int StartJobs()
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
                Trace.TraceInformation("complete {0} jobs by agent {1}", jobs.Count, _agent);
            }

            return jobs.Count;
        }

        private Task StartJobs<T>(IEnumerable<T> jobs) where T : TestJob
        {
            return Task.Run(() =>
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
                        OnError(ex);
                    }
                }
            }, _source.Token);
        }
    }
}