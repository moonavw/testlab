using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestCasesController : Controller<TestCase>
    {
        public TestCasesController(IUnitOfWork uow)
            : base(uow)
        {
        }

        public override async Task<ActionResult> Index(TestCase searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestProjectId == searchModel.TestProjectId).ToListAsync());
        }
    }
}