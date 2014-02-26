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

        public async Task<ActionResult> Index(int testprojectId)
        {
            return View(await Repo.Query().Where(z => z.TestProjectId == testprojectId).ToListAsync());
        }

        #region Overrides of Controller<TestBuild>

        [NonAction]
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        public override async Task<ActionResult> Create(TestBuild model)
        {
            var result = await base.Create(model);
            await _service.Build(model);
            return result;
        }

        #endregion
    }
}