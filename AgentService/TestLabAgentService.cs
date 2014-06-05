using System.ServiceProcess;
using TestLab.Application;

namespace TestLab.AgentService
{
    public partial class TestLabAgentService : ServiceBase
    {
        private readonly TestAgentService _service;

        public TestLabAgentService(TestAgentService service)
        {
            InitializeComponent();

            _service = service;
        }

        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }
    }
}
