using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLab.Domain.Models
{
    public class TestResult : Entity
    {
        public int TestRunId { get; set; }

        public int TestCaseId { get; set; }

        public string LogFile { get; set; }

        public string Summary { get; set; }

        public string ErrorMessage { get; set; }

        public ResultType Type { get; set; }

        public DateTime Created { get; set; }

        public virtual TestRun TestRun { get; set; }

        public virtual TestCase TestCase { get; set; }
    }
}
