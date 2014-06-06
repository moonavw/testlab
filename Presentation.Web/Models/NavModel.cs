using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using TestLab.Domain;

namespace TestLab.Presentation.Web.Models
{
    public class NavItem
    {
        public NavItem()
        {
            Children = new List<NavItem>();
            RouteValues = new RouteValueDictionary();
        }

        public string Text { get; set; }
        
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

        public List<NavItem> Children { get; set; }
    }

    public abstract class NavBase : List<NavItem>
    {
        protected NavBase() { }
    }

    public class Nav : NavBase
    {
        public Nav()
            : base()
        {
            Add(new NavItem { Text = "Dashboard", ControllerName = "Home", ActionName = "Index" });
            Add(new NavItem { Text = "Help", ControllerName = "Wiki", ActionName = "Index" });
        }
    }

    public class HomeNav : NavBase
    {
        public HomeNav()
            : base()
        {
            Add(new NavItem { Text = "Agents", ControllerName = "TestAgents", ActionName = "Index" });
            Add(new NavItem { Text = "Projects", ControllerName = "TestProjects", ActionName = "Index" });
        }
    }

    public class WikiNav : HomeNav { }

    public class TestAgentNav : HomeNav
    {
        public TestAgentNav() : base() { }

        public TestAgentNav(TestAgent agent)
            : this()
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestAgents", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.
            Add(new NavItem { Text = agent.Name, ControllerName = "TestAgents", ActionName = "show", RouteValues = new RouteValueDictionary(new { id = agent.Id }) });
        }
    }

    public class TestProjectNav : HomeNav
    {
        public TestProjectNav() : base() { }

        public TestProjectNav(TestProject proj)
            : this()
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestProjects", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.
            Add(new NavItem { Text = proj.Name, ControllerName = "TestProjects", ActionName = "show", RouteValues = new RouteValueDictionary(new { id = proj.Id }) });

            Add(new NavItem { Text = "Builds", ControllerName = "TestBuilds", ActionName = "Index", RouteValues = new RouteValueDictionary(new { testprojectId = proj.Id }) });
            Add(new NavItem { Text = "Plans", ControllerName = "TestPlans", ActionName = "Index", RouteValues = new RouteValueDictionary(new { testprojectId = proj.Id }) });
            Add(new NavItem { Text = "Sessions", ControllerName = "TestSessions", ActionName = "Index", RouteValues = new RouteValueDictionary(new { testprojectId = proj.Id }) });
        }
    }

    public class TestPlanNav : TestProjectNav
    {
        public TestPlanNav(TestProject proj) : base(proj) { }

        public TestPlanNav(TestPlan plan)
            : this(plan.Project)
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestPlans", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.
            Add(new NavItem { Text = plan.Name, ControllerName = "TestPlans", ActionName = "show", RouteValues = new RouteValueDictionary(new { id = plan.Id, testprojectId = plan.Project.Id }) });
        }
    }

    public class TestBuildNav : TestProjectNav
    {
        public TestBuildNav(TestProject proj) : base(proj) { }

        public TestBuildNav(TestBuild build) : this(build.Project) { }
    }

    public class TestSessionNav : TestProjectNav
    {
        public TestSessionNav(TestProject proj) : base(proj) { }

        public TestSessionNav(TestSession session)
            : this(session.Project)
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestSessions", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.
            Add(new NavItem { Text = session.Name, ControllerName = "TestSessions", ActionName = "show", RouteValues = new RouteValueDictionary(new { id = session.Id, testprojectId = session.Project.Id }) });
        }
    }
}