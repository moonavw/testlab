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

        public override async Task<ActionResult> Index(TestPlan searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestProjectId == searchModel.TestProjectId).ToListAsync());
        }

        public override ActionResult New(TestPlan model)
        {
            model.Project = Uow.Repository<TestProject>().Find(model.TestProjectId);
            return base.New(model);
        }

        [NonAction]
        public override Task<ActionResult> Create(TestPlan model)
        {
            return base.Create(model);
        }

        public async Task<ActionResult> Create(TestPlan model, int[] testcases)
        {
            model.Project = Uow.Repository<TestProject>().Find(model.TestProjectId);
            if (testcases == null)
            {
                ModelState.AddModelError("testcases", "no tests picked for this plan");
            }
            else
            {
                model.Cases = new HashSet<TestCase>(await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id) && z.Published != null).ToListAsync());
            }
            return await Create(model);
        }

        [NonAction]
        public override Task<ActionResult> Update(int id, TestPlan model)
        {
            return base.Update(id, model);
        }

        public async Task<ActionResult> Update(int id, TestPlan model, int[] testcases)
        {
            model.Project = Uow.Repository<TestProject>().Find(model.TestProjectId);
            if (testcases == null)
            {
                ModelState.AddModelError("testcases", "no tests picked for this plan");
            }
            else
            {
                var entity = await Repo.FindAsync(id);
                Repo.Merge(entity, model);
                model = entity;

                model.Cases.Clear();
                model.Cases = new HashSet<TestCase>(await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id) && z.Published != null).ToListAsync());
            }
            return await Update(id, model);
        }
    }
}