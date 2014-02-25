using System.Threading.Tasks;

namespace TestLab.Domain
{
    public interface ITestDeployer
    {
        Task Deploy(TestBin srcBin, TestBin destBin);
    }
}