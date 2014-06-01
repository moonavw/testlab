using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestBuildMapping : EntityTypeConfiguration<TestBuild>
    {
        public TestBuildMapping()
        {
            Map(m => m.Requires("Type").HasValue((byte)TestJobType.TestBuild));


            HasRequired(z => z.Project)
                .WithMany(f => f.Builds)
                .Map(m => m.MapKey("TestProjectId"));
        }
    }
}