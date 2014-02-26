using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Application;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestBuildsController : Controller<TestBuild>
    {
        private readonly ITestService _service;

        public TestBuildsController(IUnitOfWork uow, ITestService service)
            : base(uow)
        {
            _service = service;
        }


        public async Task<ActionResult> Start(int id)
        {
            var entity = await Repo.FindAsync(id);
            await _service.Build(entity);
            return RedirectToAction("Show", new { id });
        }

        public override async Task<ActionResult> Index(TestBuild searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestProjectId == searchModel.TestProjectId).ToListAsync());
        }
    }
}