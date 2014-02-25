using System;
using System.Threading.Tasks;
using TestLab.Domain;

namespace TestLab.Infrastructure.Ruby
{
    public class RubyTestBuilder : ITestBuilder
    {
        #region Implementation of ITestBuilder

        public TestSrcType Type
        {
            get { return TestSrcType.Ruby; }
        }

        public async Task Build(TestSrc src, TestBin bin)
        {
            await Task.Run(() =>
            {
                bin.Location = src.Location;
            });
        }

        #endregion
    }
}
