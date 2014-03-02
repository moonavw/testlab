using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

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

        public async Task<ActionResult> Index(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Project = project;
            return View(project.Plans);
        }

        public async Task<ActionResult> Show(int id, int testprojectId)
        {
            var entity = await _planRepo.FindAsync(id);
            if (entity == null || entity.Project.Id != testprojectId)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        public async Task<ActionResult> New(int testprojectId)
        {
            var model = new TestPlan { Project = await _projRepo.FindAsync(testprojectId) };
            if (model.Project == null)
            {
                return HttpNotFound();
            }
            return View(model);
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
                model.Cases = new HashSet<TestCase>(project.Cases.Where(z => testcases.Contains(z.Id)));
            }
            if (ModelState.IsValid)
            {
                project.Plans.Add(model);
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

                    entity.Cases.Clear();
                    entity.Cases = new HashSet<TestCase>(project.Cases.Where(z => testcases.Contains(z.Id)));
                    await _uow.CommitAsync();

                    return RedirectToAction("Show", new { id, testprojectId });
                }
            }
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
            return RedirectToAction("Index", new {testprojectId});
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