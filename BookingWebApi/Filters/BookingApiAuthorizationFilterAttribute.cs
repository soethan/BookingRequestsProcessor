using BookingWebApi.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BookingWebApi.Filters
{
    public class BookingApiAuthorizationFilterAttribute: AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.CurrentCultureIgnoreCase) &&
                    !string.IsNullOrEmpty(authHeader.Parameter))
                {
                    var token = authHeader.Parameter;
                       
                    if (!string.IsNullOrEmpty(token))//Actual usage: Find token in DB
                    {
                        return;
                    }
                }
            }
            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='blog' location='http://localhost/account/login'");
        }
    }
}