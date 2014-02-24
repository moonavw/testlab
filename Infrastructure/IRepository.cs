namespace TestLab.Infrastructure
{
    public interface IRepository<TEntity> : NPatterns.ObjectRelational.IRepository<TEntity> where TEntity : Entity
    {
    }
}