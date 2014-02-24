using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Application;
using TestLab.Infrastructure;
using TestLab.Domain;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestProjectsController : Controller<TestProject>
    {
        private readonly ITestService _service;

        public TestProjectsController(IUnitOfWork uow, ITestService service)
            : base(uow)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Build(int id)
        {
            var entity = await Repo.FindAsync(id);
            await _service.Build(entity);
            return RedirectToAction("Show", new { id });
        }
    }
}
