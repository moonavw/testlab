using System.Collections.Generic;
using TestLab.Infrastructure;

namespace TestLab.Domain.Services
{
    public interface ITestReporter
    {
        TestReport Report(TestRun run);
    }
}