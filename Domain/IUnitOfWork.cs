using NPatterns.ObjectRelational;

namespace TestLab.Domain
{
    public interface IUnitOfWork : NPatterns.ObjectRelational.IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : Entity;
    }
}