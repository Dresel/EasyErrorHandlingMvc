namespace EasyErrorHandlingMvc.Samples.Mvc4
{
	using System.Web.Mvc;

	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}