using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting;
using TestLab.Presentation.Web.Controllers;

[assembly: WebActivator.PostApplicationStartMethod(typeof(TestLab.Presentation.Web.RouteConfig), "Start")]

namespace TestLab.Presentation.Web
{
    public class RouteConfig
    {
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();
            //AreaRegistration.RegisterAllAreas();
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoutes<Routes>();
        }

        public static void Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }


        public class Routes : RouteSet
        {
            public override void Map(IMapper map)
            {
#if DEBUG
                map.DebugRoute("routedebug");
#endif

                map.Root<HomeController>(x => x.Index());

                map.Resource<HomeController>(home =>
                {
                    home.Only();
                    //home.Member(x => x.Get("about"));
                });

                map.Resources<TestAgentsController>(agents =>
                {
                    agents.As("agents");
                    agents.Only("index", "show");
                });

                map.Resources<TestProjectsController>(projects =>
                {
                    projects.As("projects");

                    projects.Resources<TestBuildsController>(builds =>
                    {
                        builds.As("builds");
                    });

                    projects.Resources<TestPlansController>(plans =>
                    {
                        plans.As("plans");
                    });

                    projects.Resources<TestSessionsController>(sessions =>
                    {
                        sessions.As("sessions");
                    });
                });
            }
        }

    }
}
