using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestBuildMapping : EntityTypeConfiguration<TestBuild>
    {
        public TestBuildMapping()
        {
            ToTable("TestBuilds");

            HasKey(z => z.Id);

            HasRequired(z => z.TestSource)
                .WithMany(f => f.TestBuilds)
                .HasForeignKey(z => z.TestSourceId);
        }
    }
}
