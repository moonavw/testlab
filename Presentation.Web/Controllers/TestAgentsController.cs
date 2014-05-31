using NPatterns.Messaging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;
using TestLab.Presentation.Web.Models;

namespace TestLab.Presentation.Web.Controllers
{
    public class TestAgentsController : ApplicationController
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TestAgent> _agentRepo;

        public TestAgentsController(IUnitOfWork uow)
        {
            _uow = uow;
            _agentRepo = uow.Repository<TestAgent>();
        }

        private void SetNav(TestAgent agent = null)
        {
            ViewBag.Nav = agent == null ? new TestAgentNav() : new TestAgentNav(agent);
        }

        public async Task<ActionResult> Index()
        {
            SetNav();
            return View(await _agentRepo.Query().ToListAsync());
        }

        public async Task<ActionResult> Show(int id)
        {
            var entity = await _agentRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            SetNav(entity);
            return View(entity);
        }
    }
}