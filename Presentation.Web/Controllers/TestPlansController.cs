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
    public class TestPlansController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestProject> _projRepo;
        private readonly IRepository<TestPlan> _planRepo;

        public TestPlansController(IUnitOfWork uow)
        {
            _uow = uow;
            _projRepo = uow.Repository<TestProject>();
            _planRepo = uow.Repository<TestPlan>();
        }

        private void SetNav(TestPlan plan)
        {
            ViewBag.Nav = new TestPlanNav(plan);
        }

        private void SetNav(TestProject proj)
        {
            ViewBag.Nav = new TestPlanNav(proj);
        }

        public async Task<ActionResult> Index(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            SetNav(project);
            return View(project.Plans.Actives());
        }

        public async Task<ActionResult> Show(int id, int testprojectId)
        {
            var entity = await _planRepo.FindAsync(id);
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
            return View(new TestPlan { Project = project });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int testprojectId, TestPlan model, int[] testcases)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            if (testcases == null)
            {
                ModelState.AddModelError("testcases", "no tests picked for this plan");
            }
            else
            {
                model.SetCases(project.Cases.Where(z => testcases.Contains(z.Id)));
            }
            if (ModelState.IsValid)
            {
                project.Plans.Add(model);
                await _uow.CommitAsync();
                return RedirectToAction("Show", new { id = model.Id, testprojectId });
            }
            SetNav(model);
            return View("new", model);
        }

        public Task<ActionResult> Edit(int id, int testprojectId)
        {
            return Show(id, testprojectId);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int id, int testprojectId, TestPlan model, int[] testcases)
        {
            var project = model.Project = await _projRepo.FindAsync(testprojectId);
            if (testcases == null)
            {
                ModelState.AddModelError("testcases", "no tests picked for this plan");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = project.Plans.First(z => z.Id == id);
                    _planRepo.Merge(entity, model);
                    entity.SetCases(project.Cases.Where(z => testcases.Contains(z.Id)));

                    await _uow.CommitAsync();

                    return RedirectToAction("Show", new { id, testprojectId });
                }
            }
            SetNav(model);
            return View("edit", model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Destroy(int id, int testprojectId)
        {
            var entity = await _planRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            _planRepo.Remove(entity);
            await _uow.CommitAsync();
            return RedirectToAction("Index", new { testprojectId });
        }
    }
}