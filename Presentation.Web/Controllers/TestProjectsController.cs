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
    public class TestProjectsController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<ISourcePuller> _pullers;
        private readonly IMessageBus _bus;

        public TestProjectsController(IUnitOfWork uow,
            IEnumerable<ITestDriver> drivers,
            IEnumerable<ISourcePuller> pullers,
            IMessageBus bus)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _drivers = drivers;
            _pullers = pullers;
            _bus = bus;
        }

        [HttpPost]
        public async Task<ActionResult> Build(int id)
        {
            await _bus.PublishAsync(new BuildProjectCommand(id));
            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Show", new { id });
                formats["text"] = () => Content("Started");
            });
        }

        public async Task<ActionResult> Index()
        {
            return View(await _projRepo.Query().ToListAsync());
        }

        private void SetViewData()
        {
            ViewBag.DriverNames = _drivers.Select(z => z.Name);
        }

        public async Task<ActionResult> Show(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        public ActionResult New()
        {
            SetViewData();
            return View(new TestProject());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TestProject model)
        {
            if (!_pullers.Any(z => z.CanPull(model.RepoPathOrUrl)))
            {
                ModelState.AddModelError("RepoPathOrUrl", "no puller can pull this");
            }

            if (ModelState.IsValid)
            {
                _projRepo.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id = model.Id });
            }
            SetViewData();
            return View("new", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            SetViewData();
            return View(entity);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int id, TestProject model)
        {
            if (!_pullers.Any(z => z.CanPull(model.RepoPathOrUrl)))
            {
                ModelState.AddModelError("RepoPathOrUrl", "no puller can pull this");
            }
            if (ModelState.IsValid)
            {
                _projRepo.Modify(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id });
            }
            SetViewData();
            return View("edit", model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int id)
        {
            var entity = await _projRepo.FindAsync(id);

            var sessionRepo = _uow.Repository<TestSession>();
            entity.Sessions.ToList().ForEach(z => sessionRepo.Remove(z));

            var planRepo = _uow.Repository<TestPlan>();
            entity.Plans.ToList().ForEach(z => planRepo.Remove(z));

            _projRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
