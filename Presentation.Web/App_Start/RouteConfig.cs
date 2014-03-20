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
                    home.Member(x => x.Get("about"));
                });

                map.Resources<TestProjectsController>(projects =>
                {
                    projects.As("projects");
                    projects.Member(x => x.Post("build"));

                    projects.Resources<TestPlansController>(plans =>
                    {
                        plans.As("plans");
                    });

                    projects.Resources<TestCasesController>(cases =>
                    {
                        cases.As("cases");
                        cases.Only("index");
                    });

                    projects.Resources<TestSessionsController>(sessions =>
                    {
                        sessions.As("sessions");
                        sessions.Member(x => x.Post("start"));

                        sessions.Resources<TestRunsController>(runs =>
                        {
                            runs.As("runs");
                            runs.Only("index");
                        });
                    });
                });
            }
        }

    }
}
