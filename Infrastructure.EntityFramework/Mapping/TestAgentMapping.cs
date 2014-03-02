using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EntityFramework.Mapping
{
    public class TestAgentMapping : ComplexTypeConfiguration<TestAgent>
    {
        public TestAgentMapping()
        {
            Ignore(z => z.Password);
            Property(z => z.EncryptedPassword).HasColumnName("Password");
        }
    }
}