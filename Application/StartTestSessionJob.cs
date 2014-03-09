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
            var agent = session.Agent;

            var driver = _drivers.FirstOrDefault(z => z.Name.Equals(project.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver == null) throw new NotSupportedException("no driver for this test");

            //start
            session.Started = DateTime.Now;
            await _uow.CommitAsync();

            //extract
            await _archiver.Extract(session.Build.Location, session.BuildDirOnAgent);

            //runs
            var tasks = (from t in session.Runs
                         select driver.CreateTask(t)).ToList();

            using (var ts = new TS.TaskService(agent.Server, agent.UserName, agent.Domain, agent.Password))
            {
                foreach (var t in tasks)
                {
                    var td = ts.NewTask();
                    td.Actions.Add(new TS.ExecAction(t.StartProgram, t.StartProgramArgs));
                    ts.RootFolder
                      .RegisterTaskDefinition(t.Name, td, TS.TaskCreation.CreateOrUpdate,
                                              agent.DomainUser, agent.Password, TS.TaskLogonType.Password)
                      .Run();

                    t.Run.Started = DateTime.Now;
                }
                await _uow.CommitAsync();

                //wait for result
                var waitings = (from t in tasks
                                let st = ts.GetTask(t.Name)
                                select Task.Run(async () =>
                                {
                                    do
                                    {
                                        Thread.Sleep(1000);
                                    }
                                    while (st.State == TS.TaskState.Running);
                                    st.TaskService.RootFolder.DeleteTask(t.Name);

                                    t.Run.Result = await driver.ParseResult(t);

                                    t.Run.Completed = DateTime.Now;
                                    await _uow.CommitAsync();
                                })).ToArray();

                await Task.WhenAll(waitings);
            }

            session.Completed = DateTime.Now;
            await _uow.CommitAsync();
        }
    }
}