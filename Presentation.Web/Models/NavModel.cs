using System;
using System.Collections.Generic;
using System.Linq;
using TestLab.Domain;
using System.Linq;

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

        public List<NavItem> Children { get; set; }
    }

    public class HomeNav : List<NavItem>
    {
        public HomeNav()
        {
            Add(new NavItem { Text = "Agents", ControllerName = "TestAgents", ActionName = "Index" });
            Add(new NavItem { Text = "Projects", ControllerName = "TestProjects", ActionName = "Index", Children = new List<NavItem> { new NavItem { Text = "Create New", ControllerName = "TestProjects", ActionName = "new" } } });
        }
    }

    public class TestAgentNav : HomeNav
    {
        private readonly NavItem itemGroup;

        public TestAgentNav()
            : base()
        {
            itemGroup = Find(z => z.ControllerName.Equals("TestAgents", StringComparison.OrdinalIgnoreCase));
        }

        public TestAgentNav(TestAgent agent)
            : this()
        {
            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", agent.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = agent.Id }
            });
        }
    }

    public class TestProjectNav : HomeNav
    {
        public TestProjectNav() : base() { }

        public TestProjectNav(TestProject proj)
            : this()
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestProjects", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", proj.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = proj.Id }
            });

            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", proj.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = proj.Id }
            });

            Add(new NavItem { Text = "Builds", ControllerName = "TestBuilds", ActionName = "Index", RouteValues = new { testprojectId = proj.Id }, Children = new List<NavItem> { new NavItem { Text = "Create New", ControllerName = "TestBuilds", ActionName = "new", RouteValues = new { testprojectId = proj.Id } } } });
            Add(new NavItem { Text = "Plans", ControllerName = "TestPlans", ActionName = "Index", RouteValues = new { testprojectId = proj.Id }, Children = new List<NavItem> { new NavItem { Text = "Create New", ControllerName = "TestPlans", ActionName = "new", RouteValues = new { testprojectId = proj.Id } } } });
            Add(new NavItem { Text = "Sessions", ControllerName = "TestSessions", ActionName = "Index", RouteValues = new { testprojectId = proj.Id }, Children = new List<NavItem> { new NavItem { Text = "Create New", ControllerName = "TestSessions", ActionName = "new", RouteValues = new { testprojectId = proj.Id } } } });
        }
    }

    public class TestPlanNav : TestProjectNav
    {
        public TestPlanNav(TestProject proj) : base(proj) { }

        public TestPlanNav(TestPlan plan)
            : this(plan.Project)
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestPlans", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", plan.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = plan.Id, testprojectId = plan.Project.Id }
            });

            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", plan.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = plan.Id, testprojectId = plan.Project.Id }
            });
        }
    }

    public class TestBuildNav : TestProjectNav
    {
        public TestBuildNav(TestProject proj) : base(proj) { }

        public TestBuildNav(TestBuild build)
            : this(build.Project)
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestBuilds", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", build.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = build.Id, testprojectId = build.Project.Id }
            });

            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", build.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = build.Id, testprojectId = build.Project.Id }
            });
        }
    }

    public class TestSessionNav : TestProjectNav
    {
        public TestSessionNav(TestProject proj) : base(proj) { }

        public TestSessionNav(TestSession session)
            : this(session.Project)
        {
            var itemGroup = Find(z => z.ControllerName.Equals("TestSessions", StringComparison.OrdinalIgnoreCase));
            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", session.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = session.Id, testprojectId = session.Project.Id }
            });

            itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", session.Name),
                ControllerName = itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = session.Id, testprojectId = session.Project.Id }
            });
        }
    }
}