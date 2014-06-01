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

    public class TestAgentNav : HomeNav
    {
        private readonly NavItem _itemGroup;

        public TestAgentNav()
            : base()
        {
            _itemGroup = Find(z => z.ControllerName.Equals("TestAgents", StringComparison.OrdinalIgnoreCase));
        }

        public TestAgentNav(TestAgent agent)
            : this()
        {
            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", agent.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = agent.Id }
            });
        }
    }

    public class TestProjectNav : HomeNav
    {
        private readonly NavItem _itemGroup;

        public TestProjectNav()
            : base()
        {
            _itemGroup = Find(z => z.ControllerName.Equals("TestProjects", StringComparison.OrdinalIgnoreCase));
            _itemGroup.Children.Add(new NavItem
            {
                Text = "Create New",
                ControllerName = _itemGroup.ControllerName,
                ActionName = "new"
            });
        }

        public TestProjectNav(TestProject proj)
            : this()
        {
            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", proj.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = proj.Id }
            });

            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", proj.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = proj.Id }
            });

            Add(new NavItem { Text = "Builds", ControllerName = "TestBuilds", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
            Add(new NavItem { Text = "Plans", ControllerName = "TestPlans", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
            Add(new NavItem { Text = "Sessions", ControllerName = "TestSessions", ActionName = "Index", RouteValues = new { testprojectId = proj.Id } });
        }
    }

    public class TestPlanNav : TestProjectNav
    {
        private readonly NavItem _itemGroup;

        public TestPlanNav(TestProject proj)
            : base(proj)
        {
            _itemGroup = Find(z => z.ControllerName.Equals("TestPlans", StringComparison.OrdinalIgnoreCase));
            _itemGroup.Children.Add(new NavItem
            {
                Text = "Create New",
                ControllerName = _itemGroup.ControllerName,
                ActionName = "new",
                RouteValues = new { testprojectId = proj.Id }
            });
        }

        public TestPlanNav(TestPlan plan)
            : this(plan.Project)
        {
            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", plan.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = plan.Id, testprojectId = plan.Project.Id }
            });

            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", plan.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = plan.Id, testprojectId = plan.Project.Id }
            });
        }
    }

    public class TestBuildNav : TestProjectNav
    {
        private readonly NavItem _itemGroup;

        public TestBuildNav(TestProject proj)
            : base(proj)
        {
            _itemGroup = Find(z => z.ControllerName.Equals("TestBuilds", StringComparison.OrdinalIgnoreCase));
            _itemGroup.Children.Add(new NavItem
            {
                Text = "Create New",
                ControllerName = _itemGroup.ControllerName,
                ActionName = "new",
                RouteValues = new { testprojectId = proj.Id }
            });
        }

        public TestBuildNav(TestBuild build)
            : this(build.Project)
        {
            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", build.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = build.Id, testprojectId = build.Project.Id }
            });

            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", build.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = build.Id, testprojectId = build.Project.Id }
            });
        }
    }

    public class TestSessionNav : TestProjectNav
    {
        private readonly NavItem _itemGroup;

        public TestSessionNav(TestProject proj)
            : base(proj)
        {
            _itemGroup = Find(z => z.ControllerName.Equals("TestSessions", StringComparison.OrdinalIgnoreCase));
            _itemGroup.Children.Add(new NavItem
            {
                Text = "Create New",
                ControllerName = _itemGroup.ControllerName,
                ActionName = "new",
                RouteValues = new { testprojectId = proj.Id }
            });
        }

        public TestSessionNav(TestSession session)
            : this(session.Project)
        {
            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Show {0}", session.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "show",
                RouteValues = new { id = session.Id, testprojectId = session.Project.Id }
            });

            _itemGroup.Children.Add(new NavItem
            {
                Text = string.Format("Edit {0}", session.Name),
                ControllerName = _itemGroup.ControllerName,
                ActionName = "edit",
                RouteValues = new { id = session.Id, testprojectId = session.Project.Id }
            });
        }
    }
}