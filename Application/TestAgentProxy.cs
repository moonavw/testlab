using NPatterns.Messaging;
using NPatterns.ObjectRelational;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestAgentProxy
    {
        private readonly IMessageBus _bus;
        private readonly ITestLabUnitOfWork _uow;

        private TestAgent _agent;

        public TestAgentProxy(ITestLabUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
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
            }

            _agent.Active();
            _agent.LastTalked = DateTime.Now;
            _uow.Commit();
        }

        public int StartJobs()
        {
            //get NotStarted jobs assigned to current agent
            var jobs = (from e in _uow.Repository<TestJob>().Query()
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
                _uow.Detach(job);
                _bus.Publish(job);
            }
        }
    }
}
