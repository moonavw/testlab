using System.Data.Entity;
using TestLab.Infrastructure.EF.Mapping;
using TestLab.Infrastructure.EF.Migrations;

namespace TestLab.Infrastructure.EF
{
    public class TestLabDbContext : DbContext
    {
        static TestLabDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TestLabDbContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TestAgentMapping());
            modelBuilder.Configurations.Add(new TestBuildMapping());
            modelBuilder.Configurations.Add(new TestResultMapping());
            modelBuilder.Configurations.Add(new TestProjectMapping());
            modelBuilder.Configurations.Add(new TestSessionMapping());
            modelBuilder.Configurations.Add(new TestPlanMapping());
            modelBuilder.Configurations.Add(new TestCaseMapping());
            modelBuilder.Configurations.Add(new TestRunMapping());
        }
    }
}