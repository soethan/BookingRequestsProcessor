using BookingWebApi.App_Start;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly ILog _log;
        public BookingApiAuthorizationFilterAttribute()
        {

        }
        public BookingApiAuthorizationFilterAttribute(ILog log)
        {
            _log = log;
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.AbsolutePath == "/api/bookingrequest/getstatusupdate")
            {
                return;
            }
            var authHeader = actionContext.Request.Headers.Authorization;
            
            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.CurrentCultureIgnoreCase) &&
                    !string.IsNullOrEmpty(authHeader.Parameter))
                {
                    var apiKey = authHeader.Parameter;

                    if (apiKey == ConfigurationManager.AppSettings["ApiKey"])
                    {
                        return;
                    }
                }
            }
            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            _log.Info("Unauthorized access");
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='alpha' location='http://localhost/account/login'");
        }
    }
}