using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcRouteLocalization
{
    public class LocalizationRedirectRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routeValues = requestContext.RouteData.Values;

            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                routeValues["culture"] = cookieLocale.Value;
            }
            else
            {
                string cultureName = "en-gb";
                var culture = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                routeValues["culture"] = cultureName;
            }
            return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
        }
    }
}
