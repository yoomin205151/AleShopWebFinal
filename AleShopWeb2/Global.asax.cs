using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AleShopWeb2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // Configura la política de cookies
            var cookiePolicy = new HttpCookie("MiCookie");
            cookiePolicy.Expires = DateTime.Now.AddMinutes(30); // Tiempo de expiración
            HttpContext.Current.Response.Cookies.Add(cookiePolicy);

            
        }
    }
}
