using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestLab.Presentation.Web.Filters
{
    public class LogErrorAttribute : FilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            var dbException = filterContext.Exception as DbEntityValidationException;
            if (dbException != null)
            {
                foreach (var valResult in dbException.EntityValidationErrors)
                {
                    foreach (var valError in valResult.ValidationErrors)
                    {
                        Trace.TraceError("prop: {0} got err: {1}", valError.PropertyName, valError.ErrorMessage);
                    }
                }
            }
            else
            {
                Trace.TraceError(filterContext.Exception.ToString());
            }
        }

        #endregion
    }
}