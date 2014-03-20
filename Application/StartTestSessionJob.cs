using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;
using TS = Microsoft.Win32.TaskScheduler;

namespace TestLab.Application
{
    public class StartTestSessionJob : IJob
    {
        private readonly IUnitOfWork _uow;
        private readonly IArchiver _archiver;
        private readonly IEnumerable<ITestDriver> _drivers;

        public StartTestSessionJob(IUnitOfWork uow,
                                   IArchiver archiver,
                                   IEnumerable<ITestDriver> drivers)
        {
            _uow = uow;
            _archiver = archiver;
            _drivers = drivers;
        }

        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int sessionId = dataMap.GetInt("TestSessionId");
            int agentIndex = dataMap.GetInt("TestAgentIndex");

            Trace.TraceInformation("Instance {0} of {1} for test session {2} on Agent {3}", key, GetType().Name, sessionId, agentIndex);

            try
            {
                StartSession(sessionId, agentIndex).Wait();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        private async Task StartSession(int sessionId, int agentIndex)
        {
            var repo = _uow.Repository<TestSession>();
            var session = await repo.FindAsync(sessionId);
            var project = session.Project;

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            var agents = session.GetAgents().ToList();
            var agent = agents[agentIndex];

            //start
            Trace.TraceInformation("Start Test Session {0} on Agent {1}", session, agent);

            var trRepo = _uow.Repository<TestRun>();
            var pendingRuns = from r in trRepo.Query()
                              where r.TestSessionId == sessionId && r.Started == null
                              select r;


            //extract
            Trace.TraceInformation("Extract Build to {0}", agent.Server);
            await _archiver.Extract(session.Build.Location, agent.GetBuildDir(session.Build));

            TestRun run;
            while ((run = pendingRuns.FirstOrDefault()) != null)
            {
                try
                {
                    var t = driver.CreateTask(run, agent);
                    Trace.TraceInformation("Start TestRunTask {0}", t);

                    StartTestRunTask(t);

                    run.Started = DateTime.Now;
                    await _uow.CommitAsync();

                    WaitForTestRunTask(t);

                    run.Result = await driver.ParseResult(t);
                    run.Completed = DateTime.Now;

                    await _uow.CommitAsync();
                    Trace.TraceInformation("Completed TestRunTask {0}", t);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
            session.Completed = DateTime.Now;
            await _uow.CommitAsync();
            Trace.TraceInformation("Completed TestSession {0} on Agent {1}", session, agent);
        }

        private static void WaitForTestRunTask(TestRunTask t)
        {
            var agent = t.Agent;
            bool completed = false;
            do
            {
                Thread.Sleep(5000);
                using (var ts = new TS.TaskService(agent.Server, agent.UserName, agent.Domain, agent.Password))
                {
                    var st = ts.GetTask(t.Name);
                    if (st.State != TS.TaskState.Running)
                    {
                        st.TaskService.RootFolder.DeleteTask(t.Name);
                        completed = true;
                    }
                }
            } while (!completed);
        }

        private static void StartTestRunTask(TestRunTask t)
        {
            var agent = t.Agent;
            using (var ts = new TS.TaskService(agent.Server, agent.UserName, agent.Domain, agent.Password))
            {
                var td = ts.NewTask();
                td.Actions.Add(new TS.ExecAction(t.StartProgram, t.StartProgramArgs));
                ts.RootFolder
                  .RegisterTaskDefinition(t.Name, td, TS.TaskCreation.CreateOrUpdate,
                                          agent.DomainUser, agent.Password, TS.TaskLogonType.Password)
                  .Run();
            }
        }
    }
}