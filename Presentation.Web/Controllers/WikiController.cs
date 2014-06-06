using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TestLab.Presentation.Web.Models;

namespace TestLab.Presentation.Web.Controllers
{
    public class WikiController : Controller
    {
        private void SetNav()
        {
            ViewBag.Nav = new WikiNav();
        }

        private FileInfo[] GetWikiFiles()
        {
            string dirPath = Server.MapPath("~/docs");
            var dir = new DirectoryInfo(dirPath);
            return dir.GetFiles("*.md");
        }

        private IEnumerable<NavItem> GetWikiNav(FileInfo[] files)
        {
            return (from f in files
                    let key = Path.GetFileNameWithoutExtension(f.Name)
                    select new NavItem
                    {
                        Text = key,
                        ActionName = "show",
                        ControllerName = "wiki",
                        RouteValues = new RouteValueDictionary(new { id = key })
                    }).ToList();
        }

        private void SetMessage(string text)
        {
            var md = new MarkdownDeep.Markdown();
            md.ExtraMode = true;
            md.SafeMode = false;
            ViewBag.Message = md.Transform(text);
        }

        public ActionResult Index()
        {
            SetNav();

            SetMessage("# Welcome to wiki for help.");
            return View(GetWikiNav(GetWikiFiles()));
        }

        public async Task<ActionResult> Show(string id)
        {
            SetNav();

            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var wikiFiles = GetWikiFiles();

            var fi = (from f in wikiFiles
                      let key = Path.GetFileNameWithoutExtension(f.Name)
                      where key.Equals(id, StringComparison.OrdinalIgnoreCase)
                      select f).FirstOrDefault();

            if (fi == null)
                return HttpNotFound();

            using (var sr = fi.OpenText())
            {
                SetMessage(await sr.ReadToEndAsync());
            }

            return View(GetWikiNav(wikiFiles));
        }
    }
}