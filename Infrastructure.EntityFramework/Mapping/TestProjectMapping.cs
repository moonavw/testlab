using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    internal class TestProjectMapping : EntityTypeConfiguration<TestProject>
    {
        public TestProjectMapping()
        {
            HasKey(z => z.Id);
        }
    }
}