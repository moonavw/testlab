using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using TestLab.Domain;
using TestLab.Infrastructure;
using TS = Microsoft.Win32.TaskScheduler;
using PagedList;

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

            Trace.TraceInformation("Instance {0} of {1} for test session {2}", key, GetType().Name, sessionId);

            StartSession(sessionId).Wait();
        }

        #endregion

        private async Task StartSession(int sessionId)
        {
            var repo = _uow.Repository<TestSession>();
            var session = await repo.FindAsync(sessionId);
            var project = session.Project;

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            //start
            Trace.TraceInformation("Start Test Session {0}", session);
            session.Started = DateTime.Now;
            await _uow.CommitAsync();

            var agents = session.GetAgents().ToList();
            foreach (var agent in agents)
            {
                //extract
                Trace.TraceInformation("Extract Build to {0}", agent.Server);
                await _archiver.Extract(session.Build.Location, agent.GetBuildDir(session.Build));
            }

            //runs
            int page = 1;
            while (true)
            {
                var pagedRuns = session.Runs.ToPagedList(page++, agents.Count);

                var tasks = new List<TestRunTask>();
                for (int i = 0; i < pagedRuns.Count; i++)
                {
                    tasks.Add(driver.CreateTask(pagedRuns[i], agents[i]));
                }

                foreach (var t in tasks)
                {
                    Trace.TraceInformation("Start TestRunTask {0}", t);
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

                    t.Run.Started = DateTime.Now;
                }
                await _uow.CommitAsync();

                //wait for result
                var waitings = (from t in tasks
                                select Task.Run(async () =>
                                {
                                    Trace.TraceInformation("Wait for Result of TestRunTask {0}", t);
                                    var agent = t.Agent;
                                    //wait for result
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

                                    t.Run.Result = await driver.ParseResult(t);

                                    t.Run.Completed = DateTime.Now;
                                    await _uow.CommitAsync();
                                })).ToArray();

                await Task.WhenAll(waitings);
                Trace.TraceInformation("Completed {0} TestRun(s)", pagedRuns.Count);

                if (!pagedRuns.HasNextPage) break;
            }
            session.Completed = DateTime.Now;
            await _uow.CommitAsync();
            Trace.TraceInformation("Completed TestSession {0}", session);
        }
    }
}