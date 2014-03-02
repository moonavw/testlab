using System.Web.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(TestLab.Presentation.Web.FilterConfig), "Start")]

namespace TestLab.Presentation.Web
{
    public class FilterConfig
    {
        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }

        public static void Start()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
