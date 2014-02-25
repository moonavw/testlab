using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    internal class TestResultMapping : EntityTypeConfiguration<TestResult>
    {
        public TestResultMapping()
        {
            HasKey(z => new { z.TestCaseId, z.TestSessionId });
            Property(z => z.TestCaseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(z => z.TestSessionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.Case)
                .WithMany()
                .HasForeignKey(z => z.TestCaseId)
                .WillCascadeOnDelete(false);

            HasRequired(z => z.Session)
                .WithMany(f => f.Results)
                .HasForeignKey(z => z.TestSessionId);
        }
    }
}