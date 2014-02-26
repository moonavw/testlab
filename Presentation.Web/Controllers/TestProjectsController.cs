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
        public TestProjectsController(IUnitOfWork uow)
            : base(uow)
        {
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
