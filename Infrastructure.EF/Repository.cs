namespace TestLab.Infrastructure.EF
{
    public class Repository<TEntity> : NPatterns.ObjectRelational.EF.Repository<TEntity>,
        IRepository<TEntity> where TEntity : Entity
    {
        public Repository(TestLabDbContext context)
            : base(context)
        {
        }
    }
}