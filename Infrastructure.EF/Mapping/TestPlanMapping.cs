using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestPlanMapping : EntityTypeConfiguration<TestPlan>
    {
        public TestPlanMapping()
        {
            HasKey(z => z.Id);

            HasMany(z => z.Cases)
                .WithMany(f => f.Plans)
                .Map(m =>
                         m.MapLeftKey("TestPlanId")
                          .MapRightKey("TestCaseId")
                          .ToTable("TestPlans_TestCases")
                );

            HasRequired(z => z.Project)
                .WithMany(f => f.Plans)
                //.HasForeignKey(z => z.TestProjectId);
                .Map(m => m.MapKey("TestProjectId"))
                .WillCascadeOnDelete(false);
        }
    }
}