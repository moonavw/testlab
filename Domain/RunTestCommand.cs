namespace TestLab.Domain
{
    public class RunTestCommand : StartTestSessionCommand
    {
        public RunTestCommand(int testCaseId, int testSessionId)
            : base(testSessionId)
        {
            TestCaseId = testCaseId;
        }

        public int TestCaseId { get; private set; }
    }
}
