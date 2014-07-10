using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.EF;
using System.Data.Entity.Infrastructure;

namespace TestLab.Infrastructure.EF
{
    public class TestLabUnitOfWork : UnitOfWork, ITestLabUnitOfWork
    {
        public TestLabUnitOfWork()
            : base(new TestLabDbContext())
        {
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new Repository<TEntity>(Context);
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : Entity
        {
            ((IObjectContextAdapter)Context).ObjectContext.Detach(entity);
        }
    }
}