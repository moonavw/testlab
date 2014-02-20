using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestResultMapping : EntityTypeConfiguration<TestResult>
    {
        public TestResultMapping()
        {
            ToTable("TestRuns");

            HasKey(z => new { z.TestCaseId, z.TestRunId });
            Property(z => z.TestCaseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(z => z.TestRunId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.TestRun)
                .WithMany(f => f.TestResults)
                .HasForeignKey(z => z.TestRunId);

            HasRequired(z => z.TestCase)
                .WithMany(f => f.TestResults)
                .HasForeignKey(z => z.TestCaseId);
        }
    }
}
