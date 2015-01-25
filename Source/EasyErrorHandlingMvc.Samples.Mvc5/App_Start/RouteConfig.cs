namespace EasyErrorHandlingMvc.Samples.Mvc5
{
	using System.Web.Mvc;
	using System.Web.Routing;

	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.RegisterSpecificErrorHandlingRoutes();

			routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

			routes.RegisterCatchAllErrorHandlingRoute();
		}
	}
}