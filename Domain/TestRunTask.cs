using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestRunTask
    {
        public TestRun Run { get; set; }

        public TestAgent Agent { get; set; }

        public string Name
        {
            get { return string.Format("{0}_{1}_{2}_{3}", Constants.AppName, Run.Session.Project.Id, Run.Session.Id, Run.Case.Id); }
        }

        //public string Description
        //{
        //    get { return string.Format("{0}_{1}", Run.Session, Run.Case.Name); }
        //}

        public string StartProgram { get; set; }

        public string StartProgramArgs { get; set; }

        public string OutputFile { get; set; }

        public override string ToString()
        {
            return string.Format("{0} via {1}", Name, Agent);
        }
    }
}