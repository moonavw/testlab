using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public abstract class Controller<T> : ApplicationController where T : Entity, new()
    {
        protected IRepository<T> Repo { get; private set; }

        protected Controller(IUnitOfWork uow)
            : base(uow)
        {
            Repo = uow.Repository<T>();
        }

        public virtual async Task<ActionResult> Index(T searchModel)
        {
            return View(await Repo.Query().ToListAsync());
        }

        public virtual async Task<ActionResult> Show(int id)
        {
            T entity = await Repo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        public virtual ActionResult New(T model)
        {
            return View(model ?? new T());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create(T model)
        {
            if (ModelState.IsValid)
            {
                Repo.Add(model);
                await Uow.CommitAsync();
                return RedirectToAction("Index");
            }

            return View("new", model);
        }

        public virtual async Task<ActionResult> Edit(int id)
        {
            T entity = await Repo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            return View(entity);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Update(int id, T model)
        {
            if (ModelState.IsValid)
            {
                Repo.Modify(model);
                await Uow.CommitAsync();
                return RedirectToAction("Show", new { id });
            }
            return View("edit", model);
        }

        [HttpDelete]
        public virtual async Task<ActionResult> Destroy(int id)
        {
            T entity = await Repo.FindAsync(id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            Repo.Remove(entity);
            await Uow.CommitAsync();
            return RedirectToAction("Index");
        }
    }
}