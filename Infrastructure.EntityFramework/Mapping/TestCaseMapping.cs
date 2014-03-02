using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    internal class TestCaseMapping : EntityTypeConfiguration<TestCase>
    {
        public TestCaseMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Project)
                .WithMany(f => f.Cases)
                //.HasForeignKey(z => z.TestProjectId);
                .Map(m => m.MapKey("TestProjectId"));
        }
    }
}