using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    class TestCaseMapping : EntityTypeConfiguration<TestCase>
    {
        public TestCaseMapping()
        {
            ToTable("TestCases");

            HasKey(z => z.Id);

            HasRequired(z => z.TestSource)
                .WithMany(f => f.TestCases)
                .HasForeignKey(z => z.TestSourceId);
        }
    }
}
