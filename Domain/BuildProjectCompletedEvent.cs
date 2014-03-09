namespace TestLab.Domain
{
    public class BuildProjectCompletedEvent : BuildProjectCommand
    {
        public BuildProjectCompletedEvent(int testProjectId)
            : base(testProjectId)
        {
        }
    }
}
