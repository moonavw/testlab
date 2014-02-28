﻿using System.Data.Entity;
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

        [HttpPost]
        public async Task<ActionResult> Start(int id)
        {
            var entity = await Repo.FindAsync(id);
            await _service.Run(entity);
            Repo.Modify(entity);
            await Uow.CommitAsync();
            return RespondTo(formats =>
            {
                formats.Default = RedirectToAction("Index");
                formats["text"] = () => Content(entity.Started.ToString());
            });
        }

        public override async Task<ActionResult> Index(TestSession searchModel)
        {
            return View(await Repo.Query().Where(z => z.TestPlanId == searchModel.TestPlanId).ToListAsync());
        }
    }
}