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
        private readonly IMessageBus _bus;

        public TestSessionsController(IUnitOfWork uow, IMessageBus bus)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _sessionRepo = uow.Repository<TestSession>();
            _bus = bus;
        }

        [HttpPost]
        public async Task<ActionResult> Start(int id, int testprojectId)
        {
            await _bus.PublishAsync(new StartTestSessionCommand(id));
            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Show", new { id, testprojectId });
                formats["text"] = () => Content("Done");
            });
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
            model.Build = model.Project.Build;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int testprojectId, TestSession model, int testplanId)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            var plan = project.Plans.FirstOrDefault(z => z.Id == testplanId);
            if (plan == null)
            {
                ModelState.AddModelError("testplanId", "no test plan found for this test session");
            }
            else
            {
                var tests = plan.Cases.Where(z => z.Published != null).ToList();
                if (tests.Count == 0)
                {
                    ModelState.AddModelError("testplanId", "no published tests in this test plan for this test session");
                }
                else
                {
                    model.Runs = new HashSet<TestRun>(tests.Select(z => new TestRun { Case = z }));
                }
            }
            if (model.Build.Completed == null)
            {
                ModelState.AddModelError("Build.Name", "no completed build for this test session");
            }

            if (ModelState.IsValid)
            {
                project.Sessions.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id = model.Id, testprojectId });
            }

            return View("new", model);
        }

        public Task<ActionResult> Edit(int id, int testprojectId)
        {
            return Show(id, testprojectId);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int id, int testprojectId, TestSession model)
        {
            if (ModelState.IsValid)
            {
                _sessionRepo.Modify(model);
                await _uow.CommitAsync();

                return RedirectToAction("Show", new { id, testprojectId });
            }
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