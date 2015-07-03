using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcRouteLocalization
{
    public class LocalizedRouteHandler : MvcRouteHandler
    {
        protected override System.Web.IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            var urlLocale = requestContext.RouteData.Values["culture"] as string;
            var cultureName = urlLocale ?? "";
      
            if (cultureName == "")
            {
                return GetDefaultLocaleRedirectHandler(requestContext);
            }

            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // if CultureInfo.GetCultureInfo throws exception
                // we should redirect with default locale
                return GetDefaultLocaleRedirectHandler(requestContext);
            }
            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                //Save last accessed locale value in Cookie
                cookieLocale.Value = cultureName;
                requestContext.HttpContext.Response.AppendCookie(cookieLocale);
            }
            else
            {
                requestContext.HttpContext.Response.AppendCookie(new HttpCookie("locale", cultureName));
            }
            return base.GetHttpHandler(requestContext);
        }

        private static IHttpHandler GetDefaultLocaleRedirectHandler(RequestContext requestContext)
        {
            var uiCulture = CultureInfo.CurrentUICulture;
            var routeValues = requestContext.RouteData.Values;
            routeValues["culture"] = uiCulture.Name;
            return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
        }
    }
}