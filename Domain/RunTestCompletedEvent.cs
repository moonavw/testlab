namespace TestLab.Domain
{
    public class RunTestCompletedEvent : RunTestCommand
    {
        public RunTestCompletedEvent(int testCaseId, int testSessionId)
            : base(testCaseId, testSessionId)
        {
        }
    }
}