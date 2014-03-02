﻿using System.Collections.Generic;
using TestLab.Application;
using TestLab.Infrastructure;
using TestLab.Domain;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestProjectsController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IEnumerable<ITestDriver> _drivers;
        private readonly ITestService _service;

        public TestProjectsController(IUnitOfWork uow, IEnumerable<ITestDriver> drivers, ITestService service)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _drivers = drivers;
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Build(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            await _service.Build(entity);
            _projRepo.Modify(entity);
            await _uow.CommitAsync();
            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Show", new { id });
                formats["text"] = () => Content(entity.Build.Started.ToString());
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
            if (ModelState.IsValid)
            {
                _projRepo.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new {id = model.Id});
            }

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
            if (ModelState.IsValid)
            {
                _projRepo.Modify(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id });
            }
            return View("edit", model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int id)
        {
            var entity = await _projRepo.FindAsync(id);
            entity.Sessions.Clear();
            entity.Plans.Clear();
            entity.Cases.Clear();
            _projRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _uow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
