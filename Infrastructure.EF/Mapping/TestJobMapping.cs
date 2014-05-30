using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestJobMapping : EntityTypeConfiguration<TestJob>
    {
        public TestJobMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Project)
               .WithMany(f => f.Jobs)
               .Map(m => m.MapKey("TestProjectId"));

            HasOptional(z => z.Agent)
                .WithMany(f => f.Jobs)
                .Map(m => m.MapKey("TestAgentId"));
        }
    }
}
