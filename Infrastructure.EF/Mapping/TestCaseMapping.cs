using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestCaseMapping : EntityTypeConfiguration<TestCase>
    {
        public TestCaseMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Project)
                .WithMany(f => f.Cases)
                .Map(m => m.MapKey("TestProjectId"));
        }
    }
}