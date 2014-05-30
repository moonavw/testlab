using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Application
{
    public class TestQueueJobHandler : TestJobHandlerBase<TestQueue>
    {
        private readonly IArchiver _archiver;

        public TestQueueJobHandler(IUnitOfWork uow, IArchiver archiver)
            : base(uow)
        {
            _archiver = archiver;
        }

        protected override async Task Run(TestQueue job)
        {
            var session = job.Session;
            var build = session.Build;
            var agent = job.Agent;

            //start
            Debug.WriteLine("Start TestSession {0} on Agent {1}", session, agent);

            var trRepo = Uow.Repository<TestRun>();
            var pendingRuns = from r in trRepo.Query()
                              where r.Session.Id == session.Id && r.Agent == null
                              select r;

            //extract
            Trace.TraceInformation("Extract TestBuild to agent {0}", agent.Name);
            await _archiver.Extract(build.SharedPath, build.LocalPath);

            TestRun run;
            while ((run = pendingRuns.FirstOrDefault()) != null)
            {
                run.Agent = agent;
                await Uow.CommitAsync();

                //wait for job done
                while (trRepo.Query().Any(z => z.Id == run.Id && z.Completed == null))
                {
                    Thread.Sleep(Constants.AGENT_KEEPALIVE * 1000);
                }
            }
        }
    }
}