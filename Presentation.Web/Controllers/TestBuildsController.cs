using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;
using TestLab.Presentation.Web.Models;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestBuildsController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IRepository<TestBuild> _buildRepo;
        private readonly IRepository<TestAgent> _agentRepo;

        public TestBuildsController(IUnitOfWork uow)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _buildRepo = uow.Repository<TestBuild>();
            _agentRepo = uow.Repository<TestAgent>();
        }

        private void SetNav(TestBuild build)
        {
            ViewBag.Nav = new TestBuildNav(build);
        }

        private void SetNav(TestProject proj)
        {
            ViewBag.Nav = new TestBuildNav(proj);
        }

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
            SetNav(project);
            ViewBag.Project = project;
            return View(project.Builds);
        }

        public async Task<ActionResult> Show(int id, int testprojectId)
        {
            var entity = await _buildRepo.FindAsync(id);
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
            return View(new TestBuild { Project = project });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int testprojectId, TestBuild model, int? testagentId)
        {
            model.Project = await _projRepo.FindAsync(testprojectId);
            if (testagentId.HasValue)
            {
                model.Agent = await _agentRepo.FindAsync(testagentId);
            }
            if (ModelState.IsValid)
            {
                _buildRepo.Add(model);
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
        public async Task<ActionResult> Update(int id, int testprojectId, TestBuild model, int? testagentId)
        {
            model.Project = await _projRepo.FindAsync(testprojectId);
            if (testagentId.HasValue)
            {
                model.Agent = await _agentRepo.FindAsync(testagentId);
            }

            if (ModelState.IsValid)
            {
                _buildRepo.Modify(model);
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
            var entity = await _buildRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            _buildRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index", new { testprojectId });
        }
    }
}