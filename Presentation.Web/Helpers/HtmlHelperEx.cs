using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestLab.Presentation.Web.Helpers
{
    public static class HtmlHelperEx
    {
        public static bool IsRouteMatch(this HtmlHelper helper, string controllerName, string actionName = null, string id = null, string areaName = null)
        {
            bool matched = controllerName.Equals((string)helper.ViewContext.RouteData.Values["controller"], StringComparison.OrdinalIgnoreCase);

            if (actionName != null)
            {
                matched = matched && actionName.Equals((string)helper.ViewContext.RouteData.Values["action"], StringComparison.OrdinalIgnoreCase);
            }
            if (id != null)
            {
                matched = matched && id.Equals((string)helper.ViewContext.RouteData.Values["id"], StringComparison.OrdinalIgnoreCase);
            }
            if (areaName != null)
            {
                matched = matched && areaName.Equals((string)helper.ViewContext.RouteData.Values["area"], StringComparison.OrdinalIgnoreCase);
            }

            return matched;
        }
    }
}