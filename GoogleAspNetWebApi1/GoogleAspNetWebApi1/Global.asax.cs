﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using System.Configuration;

namespace GoogleAspNetWebApi1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Configure Stackdriver Logging via Log4Net
            var section = (XmlElement)ConfigurationManager.GetSection("log4net");
            XmlElement element =
                (XmlElement)section.GetElementsByTagName("projectId").Item(0);
            string projectId = element?.Attributes["value"].Value;
            // Configure logging only if projectId has been changed from placeholder value. 
            if (projectId != ("YOUR-PROJECT-ID"))
            {
                // Configure log4net to use Stackdriver logging from the XML configuration file.
                log4net.Config.XmlConfigurator.Configure();
            }

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "POST, PUT, DELETE");

                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
    }
}
