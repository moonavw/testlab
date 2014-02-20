using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Infrastructure.EntityFramework.Mapping
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