using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestCasesController : ApplicationController
    {
        private readonly IRepository<TestProject> _projRepo;

        public TestCasesController(IUnitOfWork uow)
        {
            _projRepo = uow.Repository<TestProject>();
        }

        public async Task<ActionResult> Index(int testprojectId)
        {
            var project = await _projRepo.FindAsync(testprojectId);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Project = project;
            return View(project.Cases);
        }
    }
}