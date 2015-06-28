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
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
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