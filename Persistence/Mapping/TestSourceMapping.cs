using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestSourceMapping : EntityTypeConfiguration<TestSource>
    {
        public TestSourceMapping()
        {
            ToTable("TestSources");

            HasKey(z => z.Id);

            Property(z => z.Name).IsRequired();
            Property(z => z.SourcePath).IsRequired();
        }
    }
}
