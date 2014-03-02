using System;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestBuild : ValueObject, IStartable
    {
        public string Name { get; set; }

        public string Location
        {
            get { return Path.Combine(Constants.BUILD_ROOT, Name); }
        }

        public DateTime? Archived { get; set; }

        #region Implementation of IStartable

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }

        #endregion
    }
}