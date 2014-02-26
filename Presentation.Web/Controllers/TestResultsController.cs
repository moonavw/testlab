using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestResultsController : Controller<TestResult>
    {
        public TestResultsController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public override async Task<ActionResult> Index(TestResult searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestSessionId == searchModel.TestSessionId).ToListAsync());
        }
    }
}