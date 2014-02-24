using System;
using System.Web.Mvc;
using RestfulRouting.Format;
using TestLab.Infrastructure;

namespace TestLab.Presentation.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        protected IUnitOfWork Uow { get; private set; }

        protected ApplicationController(IUnitOfWork uow)
        {
            Uow = uow;
        }

        protected ActionResult RespondTo(Action<FormatCollection> format)
        {
            return new FormatResult(format);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Uow.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}