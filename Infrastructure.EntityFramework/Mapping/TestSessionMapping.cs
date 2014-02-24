using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    internal class TestSessionMapping : EntityTypeConfiguration<TestSession>
    {
        public TestSessionMapping()
        {
            HasKey(z => z.Id);

            HasRequired(z => z.Plan)
                .WithMany(f => f.Sessions)
                .HasForeignKey(z => z.TestPlanId);
        }
    }
}