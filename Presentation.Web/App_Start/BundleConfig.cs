using System.Web.Optimization;

[assembly: WebActivator.PostApplicationStartMethod(typeof(TestLab.Presentation.Web.BundleConfig), "Start")]

namespace TestLab.Presentation.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        private static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      //"~/Scripts/bootstrap-*",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Content/bootstrap-*",
                      "~/Content/gridmvc.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/gridmvc").Include(
                "~/Scripts/gridmvc.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                "~/Scripts/site.js"));
        }

        public static void Start()
        {
            RegisterBundles(BundleTable.Bundles);
        }
    }
}
