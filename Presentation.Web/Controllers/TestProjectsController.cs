using System.Collections.Generic;
using TestLab.Infrastructure;
using TestLab.Domain;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestProjectsController : Controller<TestProject>
    {
        private readonly IEnumerable<ITestDriver> _drivers;

        public TestProjectsController(IUnitOfWork uow, IEnumerable<ITestDriver> drivers)
            : base(uow)
        {
            _drivers = drivers;
        }

        protected override void SetViewData(TestProject editModel)
        {
            base.SetViewData(editModel);
            ViewBag.DriverNames = _drivers.Select(z => z.Name);
        }

        public override async Task<ActionResult> Destroy(int id)
        {
            var planRepo = Uow.Repository<TestPlan>();
            var plans = await planRepo.Query().Where(z => z.TestProjectId == id).ToListAsync();
            plans.ForEach(z => planRepo.Remove(z));

            return await base.Destroy(id);
        }
    }
}
