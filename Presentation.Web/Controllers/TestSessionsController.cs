using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Application;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestSessionsController : Controller<TestSession>
    {
        private readonly ITestService _service;

        public TestSessionsController(IUnitOfWork uow, ITestService service)
            : base(uow)
        {
            _service = service;
        }

        public async Task<ActionResult> Run(int id)
        {
            var entity = await Repo.FindAsync(id);
            await _service.Run(entity);
            return RedirectToAction("Show", new { id });
        }

        public async Task<ActionResult> Index(int testplanId)
        {
            return View(await Repo.Query().Where(z => z.TestPlanId == testplanId).ToListAsync());
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