namespace SimpleErrorMVC.Helpers
{
	using System.Web.Mvc;
	using System.Web.Routing;
	using SimpleErrorMVC.Setup;

	public static class ControllerContextHelper
	{
		public static bool ParentRouteDataIsErrorController(this ControllerContext controllerContext)
		{
			RouteData routeData = controllerContext.RouteData;

			ViewContext viewContext = controllerContext.ParentActionViewContext;

			while (viewContext != null)
			{
				routeData = viewContext.RouteData;
			}

			string controller = routeData.Values["Controller"].ToString();

			return controller == Configuration.ErrorControllerName;
		}
	}
}