namespace EasyErrorHandlingMvc.Samples.Mvc5Extended
{
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;

	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			UnityConfig.RegisterComponents();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			ErrorHandlingConfig.ConfigureErrorHandling();
			LoggingConfig.ConfigureLogging();
		}
	}
}