using System.Data.Entity;
using TestLab.Domain;
using TestLab.Infrastructure.EntityFramework.Mapping;

namespace TestLab.Infrastructure.EntityFramework
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
            modelBuilder.ComplexType<TestBuild>();
            modelBuilder.ComplexType<TestResult>();
            modelBuilder.Configurations.Add(new TestProjectMapping());
            modelBuilder.Configurations.Add(new TestSessionMapping());
            modelBuilder.Configurations.Add(new TestPlanMapping());
            modelBuilder.Configurations.Add(new TestCaseMapping());
            modelBuilder.Configurations.Add(new TestRunMapping());
        }
    }
}