using System.Web.Mvc;
using TestLab.Presentation.Web.Models;

namespace TestLab.Presentation.Web.Controllers
{
    public class HomeController : Controller
    {
        private void SetNav()
        {
            ViewBag.Nav = new HomeNav();
        }

        public ActionResult Index()
        {
            SetNav();
            return View();
        }
    }
}