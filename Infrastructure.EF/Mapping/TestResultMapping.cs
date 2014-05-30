using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestResultMapping : EntityTypeConfiguration<TestResult>
    {
        public TestResultMapping()
        {
            HasKey(z => z.TestRunId)
                .Property(z => z.TestRunId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            HasRequired(z => z.Run)
                .WithOptional(f => f.Result)
                .WillCascadeOnDelete();
        }
    }
}