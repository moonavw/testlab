namespace TestLab.Infrastructure.EF
{
    public class UnitOfWork : NPatterns.ObjectRelational.EF.UnitOfWork, IUnitOfWork
    {
        public UnitOfWork()
            : base(new TestLabDbContext())
        {
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new Repository<TEntity>((TestLabDbContext) Context);
        }
    }
}