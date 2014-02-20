using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLab.Infrastructure;

namespace TestLab.Domain.Models
{
    public class TestBuild : Entity
    {
        public int Id { get; set; }

        public int TestSourceId { get; set; }

        public DateTime Created { get; set; }

        public virtual TestSource TestSource { get; set; }

        public string Name
        {
            get { return TestSource == null ? null : string.Format("{0}_{1:yyyyMMdd_hhmm}", TestSource.Name, Created); }
        }

        public string LocalPath
        {
            get { return string.IsNullOrEmpty(Name) ? null : Path.ChangeExtension(Path.Combine(Constants.BUILD_ROOT, Name), ".zip"); }
        }
    }
}
