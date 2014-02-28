using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Application;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestRunsController : Controller<TestRun>
    {
        private readonly ITestService _service;

        public TestRunsController(IUnitOfWork uow, ITestService service)
            : base(uow)
        {
            _service = service;
        }

        public override async Task<ActionResult> Index(TestRun searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestSessionId == searchModel.TestSessionId).ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Start(int id, int testsessionId)
        {
            var entity = await Repo.FindAsync(id, testsessionId);
            await _service.Run(entity);
            Repo.Modify(entity);
            await Uow.CommitAsync();
            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Index");
                formats["text"] = () => Content(entity.Started.ToString());
            });
        }
    }
}