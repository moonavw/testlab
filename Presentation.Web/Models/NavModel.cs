using System;
using System.Collections.Generic;
using System.Linq;
using TestLab.Domain;

namespace TestLab.Presentation.Web.Models
{
    public class NavItem
    {
        public NavItem()
        {
            Children = new List<NavItem>();
        }
        public string Text { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public object RouteValues { get; set; }

        public bool IsActive { get; set; }

        public List<NavItem> Children { get; private set; }
    }

    public class HomeNav : List<NavItem>
    {
        public HomeNav()
        {
            Add(new NavItem { Text = "Agents", ControllerName = "TestAgents", ActionName = "Index" });
            Add(new NavItem { Text = "Projects", ControllerName = "TestProjects", ActionName = "Index" });
        }
    }

    public class ProjectNav : HomeNav
    {
        public ProjectNav() : base() { }

        public ProjectNav(TestProject proj)
            : this()
        {
            var activeNavItem = this.Find(z => z.Text == "Projects");

            Add(new NavItem { Text = "Cases", ControllerName = "TestProjects", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
            Add(new NavItem { Text = "Plans", ControllerName = "TestPlans", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
            Add(new NavItem { Text = "Builds", ControllerName = "TestBuilds", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
            Add(new NavItem { Text = "Sessions", ControllerName = "TestSessions", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
        }
    }
}