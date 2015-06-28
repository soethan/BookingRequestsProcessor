using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using BookingWebApi.Filters;
using WebApiContrib.Formatting.Jsonp;
using System.Web.Http.Cors;

namespace BookingWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Filters.Add(new GlobalExceptionFilterAttribute());
            
            //To support CORS Cross-Origin Resource Sharing, use below
            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "User",
                routeTemplate: "api/user/login",
                defaults: new { controller = "user" }
            );

            config.Routes.MapHttpRoute(
                name: "BookingRequests",
                routeTemplate: "api/bookingrequest/{requestNumber}",
                defaults: new { controller = "BookingRequest", requestNumber = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
           
        }
    }
}
