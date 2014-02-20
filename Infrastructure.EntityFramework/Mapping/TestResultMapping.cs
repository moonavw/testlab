using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;
using TestLab.Infrastructure;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    class TestResultMapping : EntityTypeConfiguration<TestResult>
    {
        public TestResultMapping()
        {
            ToTable("TestRuns");

            HasKey(z => new { z.TestCaseId, z.TestRunId });
            Property(z => z.TestCaseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(z => z.TestRunId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.TestReport)
                .WithMany(f => f.TestResults)
                .HasForeignKey(z => z.TestRunId);

            HasRequired(z => z.TestCase)
                .WithMany(f => f.TestResults)
                .HasForeignKey(z => z.TestCaseId);
        }
    }
}
