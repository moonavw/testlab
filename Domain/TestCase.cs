using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using TestLab.Infrastructure;

namespace TestLab.Domain
{
    public class TestCase : Entity
    {
        public TestCase()
        {
            Plans = new HashSet<TestPlan>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public string Keyword { get; set; }

        public string FullName
        {
            get { return Location + "." + Name; }
        }

        public DateTime? Published { get; set; }

        public virtual TestProject Project { get; set; }

        public virtual ICollection<TestPlan> Plans { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}