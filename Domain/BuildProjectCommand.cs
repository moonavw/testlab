namespace TestLab.Domain
{
    public class BuildProjectCommand
    {
        public BuildProjectCommand(int testProjectId)
        {
            TestProjectId = testProjectId;
        }

        public int TestProjectId { get; private set; }
    }
}