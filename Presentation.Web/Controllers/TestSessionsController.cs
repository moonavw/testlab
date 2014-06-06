using NPatterns.Messaging;
using NPatterns.ObjectRelational;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;
using TestLab.Presentation.Web.Models;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestSessionsController : ApplicationController
    {
        private readonly ITestLabUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IRepository<TestSession> _sessionRepo;
        private readonly IRepository<TestAgent> _agentRepo;
        private readonly IMessageBus _bus;

        public TestSessionsController(ITestLabUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _sessionRepo = uow.Repository<TestSession>();
            _agentRepo = uow.Repository<TestAgent>();
            _bus = bus;
        }

        private void SetNav(TestSession session)
        {
            ViewBag.Nav = new TestSessionNav(session);
        }

        private void SetNav(TestProject proj)
        {
            ViewBag.Nav = new TestSessionNav(proj);
        }

        private void SetViewData()
        {
            ViewBag.Agents = from e in _agentRepo.Query().Actives()
                             where e.IsOnline
                             select e;
        }

        [HttpPost]
        public async Task<ActionResult> Restart(int id, int testprojectId)
        {
            var entity = await _sessionRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }

            //set queues incomplete to restart
            foreach (var queue in entity.Queues)
            {
                queue.Completed = null;
                //set failed runs incomplete to restart
                foreach (var run in queue.Runs.Where(z => z.Result.PassOrFail == false))
                {
                    run.Completed = null;
                    run.Result = new TestResult();
                }
            }

            await _uow.CommitAsync();

            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Show", new { id, testprojectId });
                formats["text"] = () => Content("Started");
            });
        }

        public async Task<ActionResult> Index(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            SetNav(project);
            return View(project.Sessions.Actives());
        }

        public async Task<ActionResult> Show(int id, int testprojectId)
        {
            var entity = await _sessionRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            SetNav(entity);
            return View(entity);
        }

        public async Task<ActionResult> New(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            SetNav(project);
            SetViewData();
            return View(new TestSession { Project = project });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int testprojectId, TestSession model, int testplan, int testbuild, int[] testagents)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            model.Build = project.Builds.FirstOrDefault(z => z.Id == testbuild);
            if (model.Build == null)
            {
                ModelState.AddModelError("testbuild", "no completed build for this test session");
            }
            var plan = model.Plan = project.Plans.FirstOrDefault(z => z.Id == testplan);
            if (plan == null)
            {
                ModelState.AddModelError("testplan", "no test plan found for this test session");
            }
            else
            {
                if (plan.Cases.Count == 0)
                {
                    ModelState.AddModelError("testplan", "empty test plan for this test session");
                }
            }

            if (ModelState.IsValid)
            {
                if (testagents != null)
                {
                    var agents = (from e in _agentRepo.Query()
                                  where testagents.Contains(e.Id)
                                  select e).ToList().AsReadOnly();
                    model.SetAgents(agents);
                }
                project.Sessions.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id = model.Id, testprojectId });
            }
            SetNav(model);
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
        public async Task<ActionResult> Update(int id, int testprojectId, TestSession model, int testplan, int testbuild, int[] testagents)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            model.Build = project.Builds.FirstOrDefault(z => z.Id == testbuild);
            if (model.Build == null)
            {
                ModelState.AddModelError("testbuild", "no completed build for this test session");
            }
            var plan = model.Plan = project.Plans.FirstOrDefault(z => z.Id == testplan);
            if (plan == null)
            {
                ModelState.AddModelError("testplan", "no test plan found for this test session");
            }
            else
            {
                if (plan.Cases.Count == 0)
                {
                    ModelState.AddModelError("testplan", "empty test plan for this test session");
                }
            }
            if (ModelState.IsValid)
            {
                _sessionRepo.Modify(model);
                if (testagents != null)
                {
                    var queueRepo = _uow.Repository<TestQueue>();
                    var q = await (from e in queueRepo.Query()
                                   where e.Session.Id == model.Id
                                   select e).ToListAsync();
                    q.ForEach(z => queueRepo.Remove(z));

                    var agents = (from e in _agentRepo.Query()
                                  where testagents.Contains(e.Id)
                                  select e).ToList().AsReadOnly();
                    model.SetAgents(agents);
                }

                await _uow.CommitAsync();

                return RedirectToAction("Show", new { id, testprojectId });
            }
            SetNav(model);
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