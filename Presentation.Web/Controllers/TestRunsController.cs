using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestRunsController : Controller<TestRun>
    {
        public TestRunsController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public async Task<ActionResult> Index(int testprojectId, int testsessionId)
        {
            return View(await Repo.Query().Where(z => z.TestSessionId == testsessionId).ToListAsync());
        }

        #region Overrides of Controller<TestCase>

        [NonAction]
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        #endregion
    }
}