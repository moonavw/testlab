using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestRunMapping : EntityTypeConfiguration<TestRun>
    {
        public TestRunMapping()
        {
            HasKey(z => new { z.TestCaseId, z.TestSessionId });
            Property(z => z.TestCaseId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(z => z.TestSessionId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.Case)
                .WithMany()
                .HasForeignKey(z => z.TestCaseId)
                .WillCascadeOnDelete(false);

            HasRequired(z => z.Session)
                .WithMany(f => f.Runs)
                .HasForeignKey(z => z.TestSessionId);
        }
    }
}