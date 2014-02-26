using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    internal class TestBuildMapping : EntityTypeConfiguration<TestBuild>
    {
        public TestBuildMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Project)
                .WithMany(f => f.Builds)
                .HasForeignKey(z => z.TestProjectId);
        }
    }
}