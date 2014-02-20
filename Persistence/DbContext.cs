using System.Data.Entity;
using NPatterns.ObjectRelational.EF;
using NPatterns.ObjectRelational;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence
{
    public class TestLabDbContext : DbContext
    {
        static TestLabDbContext()
        {
#if DEBUG
#warning Db is using DropCreateDatabaseIfModelChanges strategy
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TestLabDbContext>());
#else
            Database.SetInitializer<TestLabDbContext>(null);
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Mapping.TestSourceMapping());
            modelBuilder.Configurations.Add(new Mapping.TestBuildMapping());
            modelBuilder.Configurations.Add(new Mapping.TestPlanMapping());
            modelBuilder.Configurations.Add(new Mapping.TestCaseMapping());
            modelBuilder.Configurations.Add(new Mapping.TestResultMapping());
            modelBuilder.Configurations.Add(new Mapping.TestRunMapping());
        }
    }
}
