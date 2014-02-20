using NPatterns.ObjectRelational;
using NPatterns.ObjectRelational.EF;

namespace TestLab.Infrastructure.EntityFramework
{
    public class UnitOfWork : NPatterns.ObjectRelational.EF.UnitOfWork, TestLab.Infrastructure.IUnitOfWork
    {
        public UnitOfWork()
            : base(new TestLabDbContext())
        {
        }

        #region IUnitOfWork Members

        IRepository<T> Infrastructure.IUnitOfWork.Repository<T>()
        {
            return new Repository<T>(Context);
        }

        #endregion
    }
}
