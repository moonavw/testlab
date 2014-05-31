using NPatterns.Messaging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestSessionsController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IRepository<TestSession> _sessionRepo;
        private readonly IRepository<TestAgent> _agentRepo;
        private readonly IMessageBus _bus;

        public TestSessionsController(IUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _sessionRepo = uow.Repository<TestSession>();
            _agentRepo = uow.Repository<TestAgent>();
            _bus = bus;
        }

        //[HttpPost]
        //public async Task<ActionResult> Start(int id, int testprojectId)
        //{
        //var repo = _uow.Repository<TestSession>();
        //var session = await repo.FindAsync(message.TestSessionId);
        ////get incomplete runs
        //var runs = (from r in session.Runs
        //            where r.Completed == null
        //            select r).ToList();
        //if (runs.Count == 0)
        //{//get failed runs if all complete
        //    runs = (from r in session.Runs
        //            where r.Result.PassOrFail == false
        //            select r).ToList();
        //}

        //if (runs.Count == 0)
        //    return;//no available runs for this session

        ////reset session
        //session.Started = DateTime.Now;
        //session.Completed = null;
        ////reset runs          
        //foreach (var run in runs)
        //{
        //    run.Started = null;
        //    run.Completed = null;
        //    run.Result = new TestResult();
        //}
        //await _uow.CommitAsync();
        //}

        private void SetViewData()
        {
            ViewBag.Agents = from e in _agentRepo.Query().AsEnumerable()
                             where e.IsOnline
                             select e;
        }

        public async Task<ActionResult> Index(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Project = project;
            return View(project.Sessions);
        }

        public async Task<ActionResult> Show(int id, int testprojectId)
        {
            var entity = await _sessionRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        public async Task<ActionResult> New(int testprojectId)
        {
            var model = new TestSession { Project = await _projRepo.FindAsync(testprojectId) };
            if (model.Project == null)
            {
                return HttpNotFound();
            }
            SetViewData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int testprojectId, TestSession model, int testplanId, int testbuildId, int[] testagents)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            model.Build = project.Builds.FirstOrDefault(z => z.Id == testbuildId);
            if (model.Build == null)
            {
                ModelState.AddModelError("BuildId", "no completed build for this test session");
            }
            var plan = project.Plans.FirstOrDefault(z => z.Id == testplanId);
            if (plan == null)
            {
                ModelState.AddModelError("testplanId", "no test plan found for this test session");
            }
            else
            {
                model.SetPlan(plan);
                if (model.Runs.Count == 0)
                {
                    ModelState.AddModelError("testplanId", "no published tests in this test plan for this test session");
                }
            }
            if (testagents != null)
            {
                var agents = (from e in _agentRepo.Query()
                              where testagents.Contains(e.Id)
                              select e).ToList();
                model.SetAgents(agents);
            }

            if (ModelState.IsValid)
            {
                project.Sessions.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id = model.Id, testprojectId });
            }
            SetViewData();
            return View("new", model);
        }

        public Task<ActionResult> Edit(int id, int testprojectId)
        {
            SetViewData();
            return Show(id, testprojectId);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int id, int testprojectId, TestSession model, int testbuildId, int[] testagents)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            model.Build = project.Builds.FirstOrDefault(z => z.Id == testbuildId);
            if (ModelState.IsValid)
            {
                var entity = project.Sessions.First(z => z.Id == id);
                _sessionRepo.Merge(entity, model);

                if (testagents != null)
                {
                    var agents = (from e in _agentRepo.Query()
                                  where testagents.Contains(e.Id)
                                  select e).ToList();
                    entity.SetAgents(agents);
                }

                await _uow.CommitAsync();

                return RedirectToAction("Show", new { id, testprojectId });
            }
            SetViewData();
            return View("edit", model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int id, int testprojectId)
        {
            var entity = await _sessionRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            _sessionRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index", new { testprojectId });
        }
    }
}