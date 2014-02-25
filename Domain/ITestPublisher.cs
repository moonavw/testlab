using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestPublisher
    {
        TestBinType Type { get; }

        Task<IEnumerable<TestCase>> Publish(TestBin bin);
    }
}