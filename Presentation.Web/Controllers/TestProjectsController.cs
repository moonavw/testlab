using TestLab.Infrastructure;
using TestLab.Domain;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestProjectsController : Controller<TestProject>
    {
        public TestProjectsController(IUnitOfWork uow)
            : base(uow)
        {
        }
    }
}
