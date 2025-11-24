using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ReservaYa
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Register", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "DetallesEspacio",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "EspacioDetallesController", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
