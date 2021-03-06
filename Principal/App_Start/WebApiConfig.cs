﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.OData.Builder;
using PrincipalAPI.Models;

namespace PrincipalAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            // entityset<model>("controller")
            builder.EntitySet<Host>("Hosts");
            builder.EntitySet<MasterVM>("MasterVMs");
            builder.EntitySet<Metrics>("Metrics");
            builder.EntitySet<CPU>("CPUs");
            builder.EntitySet<RAM>("RAMs");
            builder.EntitySet<Notifications>("Notifications");
            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
        }

        //public static void Register(HttpConfiguration config)
        //{
        //    // Web API configuration and services
        //    // Configure Web API to use only bearer token authentication.
        //    config.SuppressDefaultHostAuthentication();
        //    config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

        //    // Web API routes
        //    config.MapHttpAttributeRoutes();

        //    config.Routes.MapHttpRoute(
        //        name: "DefaultApi",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //}
    }
}
