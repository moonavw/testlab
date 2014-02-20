using System.Collections.Generic;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public interface ITestReporter
    {
        TestReport Report(TestRun run);
    }
}