using NPatterns.ObjectRelational;

namespace TestLab.Infrastructure
{
    public interface ITestLabUnitOfWork : IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

        void Detach<TEntity>(TEntity entity) where TEntity : Entity;
    }
}