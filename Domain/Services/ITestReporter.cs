using System.Collections.Generic;
using TestLab.Domain.Models;

namespace TestLab.Domain.Services
{
    public interface ITestReporter
    {
        TestReport Report(TestRun run);
    }
}