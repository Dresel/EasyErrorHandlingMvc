# Getting Started

1) Setup / Change ErrorHandlingConfig.ConfigureErrorHandling()

2) Call ErrorHandlingConfig.ConfigureErrorHandling() within Global.asax.cs

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ErrorHandlingConfig.ConfigureErrorHandling(); // <-- Add to Global.asax.cs
        }
    }

3) Add routes.RegisterSpecificErrorHandlingRoutes() and routes.RegisterCatchAllErrorHandlingRoute() to RouteConfig.cs

    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        routes.MapMvcAttributeRoutes();

        routes.RegisterSpecificErrorHandlingRoutes(); // <-- Add specific routes before any generic route configuration

        routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

        routes.RegisterCatchAllErrorHandlingRoute(); // <-- Add catch all route after all other route configuration
    }

For more information see sample projects on https://github.com/Dresel/EasyErrorHandlingMvc