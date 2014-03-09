﻿namespace TestLab.Domain
{
    public class TestRunTask
    {
        public TestRun Run { get; set; }

        public string Name
        {
            get { return string.Format("{0}_{1}", Run.Session, Run.Case.Name); }
        }

        public string StartProgram { get; set; }

        public string StartProgramArgs { get; set; }

        public string OutputFile { get; set; }
    }
}