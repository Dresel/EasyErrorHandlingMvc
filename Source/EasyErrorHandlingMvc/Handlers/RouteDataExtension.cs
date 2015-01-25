namespace EasyErrorHandlingMvc.Handlers
{
	using System.Web.Routing;

	public static class RouteDataExtension
	{
		public static RouteData GetParentRouteData(this RouteData routeData)
		{
			return (RouteData)routeData.DataTokens["ParentActionViewContext"];
		}

		public static bool IsChildAction(this RouteData routeData)
		{
			return routeData.DataTokens.ContainsKey("ParentActionViewContext");
		}

		public static bool RouteDataIsErrorHandlingController(this RouteData routeData)
		{
			string controller = routeData.Values["Controller"].ToString();

			return controller == Configuration.ErrorHandlingControllerName;
		}
	}
}