﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TestLab.Presentation.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //return RedirectToAction("Index", "TestProjects");
        }

        public async Task<ActionResult> About()
        {
            var md = new MarkdownDeep.Markdown();
            md.ExtraMode = true;
            md.SafeMode = false;
            string filepath = Server.MapPath("~/readme.md");
            var fi = new FileInfo(filepath);
#if DEBUG
            if (!fi.Exists)
            {
                filepath = Path.Combine(fi.Directory.Parent.FullName, "readme.md");
                fi = new FileInfo(filepath);
            }
#endif
            using (var sr = fi.OpenText())
            {
                string input = await sr.ReadToEndAsync();

                ViewBag.Message = md.Transform(input);
            }
            return View();
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}