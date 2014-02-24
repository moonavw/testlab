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

        public async Task<ActionResult> New(int testplanId)
        {
            var plan = await Uow.Repository<TestPlan>().FindAsync(testplanId);
            var model = new TestSession
            {
                Plan = plan,
                Name = string.Format("run_{0}_{1}_{2:yyyyMMdd_hhmm}", plan.Project.Name, plan.Name, DateTime.Now),
                Runs = new HashSet<TestRun>(plan.Cases.Select(z => new TestRun { Case = z }))
            };
            var ss = User.Identity.Name.Split('\\');
            model.Config.RdpDomain = ss[0];
            model.Config.RdpUserName = ss[1];

            return View(model);
        }

        public async Task<ActionResult> Create(TestSession model, int[] testcases)
        {
            var cases = await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id)).ToListAsync();
            model.Runs = new HashSet<TestRun>(cases.Select(z => new TestRun { Case = z }));

            return await Create(model);
        }

        public async Task<ActionResult> Update(int id, TestSession model, int[] testcases)
        {
            var entity = await Repo.FindAsync(id);
            Repo.Merge(entity, model);
            model = entity;

            model.Runs.Clear();
            var cases = await Uow.Repository<TestCase>().Query().Where(z => testcases.Contains(z.Id)).ToListAsync();
            model.Runs = new HashSet<TestRun>(cases.Select(z => new TestRun { Case = z }));
            return await Update(id, model);
        }

        #region Overrides of Controller<TestCase>

        [NonAction]
        public override Task<ActionResult> Index()
        {
            return base.Index();
        }

        [NonAction]
        public override ActionResult New()
        {
            return base.New();
        }

        [NonAction]
        public override Task<ActionResult> Create(TestSession model)
        {
            return base.Create(model);
        }

        [NonAction]
        public override Task<ActionResult> Update(int id, TestSession model)
        {
            return base.Update(id, model);
        }

        #endregion
    }
}