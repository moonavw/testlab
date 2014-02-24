namespace TestLab.Infrastructure
{
    public interface IUnitOfWork : NPatterns.ObjectRelational.IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;
    }
}