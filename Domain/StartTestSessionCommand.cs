namespace TestLab.Domain
{
    public class StartTestSessionCommand
    {
        public StartTestSessionCommand(int testSessionId)
        {
            TestSessionId = testSessionId;
        }

        public int TestSessionId { get; private set; }
    }
}
