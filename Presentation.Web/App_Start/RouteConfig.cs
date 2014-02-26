using System.Web.Mvc;
using System.Web.Routing;
using Ninject.Planning;
using RestfulRouting;
using TestLab.Presentation.Web.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(TestLab.Presentation.Web.RouteConfig), "Start")]

namespace TestLab.Presentation.Web
{
    public class RouteConfig
    {
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoutes<Routes>();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }

        public static void Start()
        {
            //AreaRegistration.RegisterAllAreas();
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

                map.Resources<TestProjectsController>(projects =>
                {
                    projects.As("projects");

                    projects.Resources<TestBuildsController>(builds =>
                    {
                        builds.As("builds");
                        builds.Member(x => x.Post("start"));
                        builds.Except("new", "show", "edit");
                    });

                    projects.Resources<TestPlansController>(plans =>
                    {
                        plans.As("plans");
                        plans.Resources<TestSessionsController>(sessions =>
                        {
                            sessions.As("sessions");
                            sessions.Member(x => x.Post("start"));
#if !DEBUG
                            sessions.Except("edit", "update");
#endif                            
                            sessions.Resources<TestResultsController>(runs =>
                            {
                                runs.As("results");
                                runs.Only("index");
                            });
                        });
                    });
                    projects.Resources<TestCasesController>(cases =>
                    {
                        cases.As("cases");
#if !DEBUG
                        cases.Only("index");
#endif
                    });
                });
            }
        }

    }
}
