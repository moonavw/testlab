using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestReportMapping : EntityTypeConfiguration<TestReport>
    {
        public TestReportMapping()
        {
            ToTable("TestReports");

            HasKey(z => z.TestRunId)
                .Property(z => z.TestRunId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.TestRun)
                .WithOptional(f => f.TestReport)
                .WillCascadeOnDelete();
        }
    }
}