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
    public class TestProjectsController : ApplicationController
    {
        private readonly ITestLabUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly IEnumerable<IPuller> _pullers;
        private readonly IMessageBus _bus;

        public TestProjectsController(ITestLabUnitOfWork uow,
            IEnumerable<ITestDriver> drivers,
            IEnumerable<IPuller> pullers,
            IMessageBus bus)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _drivers = drivers;
            _pullers = pullers;
            _bus = bus;
        }

        private void SetNav(TestProject proj = null)
        {
            ViewBag.Nav = proj == null ? new TestProjectNav() : new TestProjectNav(proj);
        }

        private void SetViewData()
        {
            ViewBag.DriverNames = _drivers.Select(z => z.Name);
        }

        public async Task<ActionResult> Index()
        {
            SetNav();
            var list = await _projRepo.Query().ToListAsync();
            return View(list.Actives());
        }

        public async Task<ActionResult> Show(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            SetNav(entity);
            return View(entity);
        }

        public ActionResult New()
        {
            SetNav();
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
            SetNav(model);
            SetViewData();
            return View("new", model);
        }

        public Task<ActionResult> Edit(int id)
        {
            SetViewData();
            return Show(id);
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
            SetNav(model);
            SetViewData();
            return View("edit", model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            _projRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}
