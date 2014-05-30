using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestAgentMapping : EntityTypeConfiguration<TestAgent>
    {
        public TestAgentMapping()
        {
            HasKey(z => z.Id);
        }
    }
}