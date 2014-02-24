using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestPlansController : Controller<TestPlan>
    {
        public TestPlansController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public async Task<ActionResult> Index(int testprojectId)
        {
            return View(await Repo.Query().Where(z => z.TestProjectId == testprojectId).ToListAsync());
        }

        public async Task<ActionResult> New(int testprojectId)
        {
            var model = new TestPlan
            {
                Project = await Uow.Repository<TestProject>().FindAsync(testprojectId)
            };
            return View(model);
        }

        public async Task<ActionResult> Create(TestPlan model, int[] testcases)
        {
            model.Cases = new HashSet<TestCase>(await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id)).ToListAsync());
            return await Create(model);
        }

        public async Task<ActionResult> Update(int id, TestPlan model, int[] testcases)
        {
            var entity = await Repo.FindAsync(id);
            Repo.Merge(entity, model);
            model = entity;

            model.Cases.Clear();
            model.Cases = new HashSet<TestCase>(await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id)).ToListAsync());

            return await Update(id, model);
        }

        #region Overrides of Controller<TestPlan>

        [NonAction]
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        [NonAction]
        public override ActionResult New()
        {
            return base.New();
        }

        [NonAction]
        public override Task<ActionResult> Create(TestPlan model)
        {
            return base.Create(model);
        }

        [NonAction]
        public override Task<ActionResult> Update(int id, TestPlan model)
        {
            return base.Update(id, model);
        }

        #endregion
    }
}