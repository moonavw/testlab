using System.Data.Entity;

namespace TestLab.Infrastructure.EntityFramework
{
    public class Repository<TEntity> : NPatterns.ObjectRelational.EntityFramework.Repository<TEntity>,
        IRepository<TEntity> where TEntity : Entity
    {
        public Repository(DbContext context)
            : base(context)
        {
        }
    }
}