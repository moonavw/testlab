using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.EF;

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
    }
}