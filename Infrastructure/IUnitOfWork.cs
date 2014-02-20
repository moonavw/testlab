using NPatterns.ObjectRelational;

namespace TestLab.Infrastructure
{
    public interface IUnitOfWork : NPatterns.ObjectRelational.IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : Entity;
    }
}