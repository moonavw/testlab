using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestRunMapping : EntityTypeConfiguration<TestRun>
    {
        public TestRunMapping()
        {
            HasKey(z => new { z.TestCaseId, z.TestQueueId });

            Property(z => z.TestCaseId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(z => z.TestQueueId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.Case)
                .WithMany()
                .HasForeignKey(z => z.TestCaseId)
                .WillCascadeOnDelete(false);

            HasRequired(z => z.Queue)
                .WithMany(f => f.Runs)
                .HasForeignKey(z => z.TestQueueId);
        }
    }
}