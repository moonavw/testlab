namespace TestLab.Infrastructure.EntityFramework
{
    public class UnitOfWork : NPatterns.ObjectRelational.EntityFramework.UnitOfWork, IUnitOfWork
    {
        public UnitOfWork()
            : base(new TestLabDbContext())
        {
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new Repository<TEntity>(Context);
        }
    }
}