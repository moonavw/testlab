using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestLab.Domain.Models;

namespace TestLab.Infrastructure.Persistence.Mapping
{
    class TestPlanMapping : EntityTypeConfiguration<TestPlan>
    {
        public TestPlanMapping()
        {
            ToTable("TestPlans");

            HasKey(z => z.Id);

            Property(z => z.Name).IsRequired();

            HasMany(z => z.TestCases)
                .WithMany(f => f.TestPlans)
                .Map(m =>
                    m.MapLeftKey("TestPlanId")
                    .MapRightKey("TestCaseId")
                    .ToTable("TestPlanCases")
                    );
        }
    }
}
