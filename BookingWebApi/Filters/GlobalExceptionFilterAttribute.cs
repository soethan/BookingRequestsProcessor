using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace BookingWebApi.Filters
{
    /// <summary>
    /// Exception Filter to handle Validation Errors in Entity
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILog _log;
        public GlobalExceptionFilterAttribute()
        {

        }
        public GlobalExceptionFilterAttribute(ILog log)
        {
            _log = log;
        }
        public override void OnException(HttpActionExecutedContext context)
        {
            _log.Error("ERROR", context.Exception);
            if (context.Exception is DbEntityValidationException)
            {
                var dbEx = context.Exception as DbEntityValidationException;
                var errorList = new List<string>();
                foreach (DbEntityValidationResult entityErr in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError error in entityErr.ValidationErrors)
                    {
                        errorList.Add(error.ErrorMessage);
                    }
                }
                context.Response = context.Request.CreateResponse(HttpStatusCode.BadRequest, new { Errors = errorList });
            }
            else
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}