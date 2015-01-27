namespace EasyErrorHandlingMvc.Handlers
{
	using System.Web.Routing;

	public static class RouteDataExtension
	{
		public static bool IsChildAction(this RouteData routeData)
		{
			return routeData.DataTokens.ContainsKey("ParentActionViewContext");
		}
	}
}