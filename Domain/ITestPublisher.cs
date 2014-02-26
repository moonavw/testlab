using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestPublisher
    {
        TestType Type { get; }

        Task<IEnumerable<TestCase>> Publish(TestBuild build);
    }
}