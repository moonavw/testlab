using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestConfig : ValueObject
    {
        public string Key
        {
            get
            {
                return string.Format("{0}.{1}",
                    typeof(TestConfig).Name.ToLowerInvariant(),
                    Type.ToString().ToLowerInvariant());
            }
        }

        public string Value { get; set; }

        public TestConfigType Type { get; set; }
    }

    public enum TestConfigType
    {
        Text,
        Json,
        Xml
    }
}
