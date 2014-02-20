using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.EF;

namespace TestLab.Infrastructure.Persistence
{
    public class UnitOfWork : NPatterns.ObjectRelational.EF.UnitOfWork, TestLab.Domain.IUnitOfWork
    {
        public UnitOfWork()
            : base(new TestLabDbContext())
        {
        }

        #region IUnitOfWork Members

        IRepository<T> Domain.IUnitOfWork.Repository<T>()
        {
            return new Repository<T>(Context);
        }

        #endregion
    }
}
