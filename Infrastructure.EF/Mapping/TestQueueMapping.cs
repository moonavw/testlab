using System.Data.Entity.ModelConfiguration;
using TestLab.Domain;

namespace TestLab.Infrastructure.EF.Mapping
{
    internal class TestQueueMapping : EntityTypeConfiguration<TestQueue>
    {
        public TestQueueMapping()
        {
            Map(m => m.Requires("Type").HasValue(TestJobType.TestQueue));

            HasRequired(z => z.Session)
                .WithMany(f => f.Queues)
                .Map(m => m.MapKey("TestSessionId"))
                .WillCascadeOnDelete();
        }
    }
}
