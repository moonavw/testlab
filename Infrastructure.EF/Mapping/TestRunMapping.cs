using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestRunMapping : EntityTypeConfiguration<TestRun>
    {
        public TestRunMapping()
        {
            Map(m => m.Requires("Type").HasValue(TestJobType.TestRun));

            HasRequired(z => z.Session)
                .WithMany(f => f.Runs)
                .Map(m => m.MapKey("TestSessionId"));

            HasRequired(z => z.Case)
                .WithMany()
                .Map(m => m.MapKey("TestCaseId"))
                .WillCascadeOnDelete(false);
        }
    }
}