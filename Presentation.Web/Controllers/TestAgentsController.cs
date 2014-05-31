using NPatterns.Messaging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Domain;
using TestLab.Infrastructure;

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

        public async Task<ActionResult> Index()
        {
            return View(await _agentRepo.Query().ToListAsync());
        }

        public async Task<ActionResult> Show(int id)
        {
            var entity = await _agentRepo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }
    }
}